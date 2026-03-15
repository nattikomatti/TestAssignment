using ApprovalFlow.Data;
using ApprovalFlow.Integrations;
using ApprovalFlow.Models.Requests;
using ApprovalFlow.Models.Responses;
using ApprovalFlow.Repositories.Interfaces;
using ApprovalFlow.Services.Interfaces;
using ApprovalFlow.Workflow;

namespace ApprovalFlow.Services
{
    public class ApprovalService(
        IApprovalRepository approvalRepo,
        IApprovalHistoryRepository historyRepo,
        ApprovalStateMachine stateMachine,
        ERPClient erpClient) : IApprovalService
    {
        public ApprovalResponse Create(CreateApprovalRequest request)
        {
            var entity = new ApprovalRequest
            {
                Title = request.Title,
                Description = request.Description,
                RequestedBy = request.RequestedBy,
                Steps =
                [
                    new() { RequestId = default, StepOrder = 1, ApproverRole = "Manager" },
                    new() { RequestId = default, StepOrder = 2, ApproverRole = "Director" },
                    new() { RequestId = default, StepOrder = 3, ApproverRole = "Finance" },
                ]
            };

            foreach (var step in entity.Steps)
                step.RequestId = entity.Id;

            approvalRepo.Add(entity);

            return new ApprovalResponse
            {
                Success = true,
                Message = "สร้างคำขออนุมัติสำเร็จ",
                Data = MapToDetail(entity)
            };
        }

        public ApprovalResponse Approve(Guid requestId, ApproveRequest request)
        {
            var entity = approvalRepo.GetById(requestId);
            if (entity is null)
                return new ApprovalResponse { Success = false, Message = "ไม่พบคำขออนุมัติ" };

            var currentStep = stateMachine.GetCurrentPendingStep(entity.Steps);
            if (currentStep is null)
                return new ApprovalResponse { Success = false, Message = "ไม่มีขั้นตอนที่รอการอนุมัติ" };

            if (!currentStep.ApproverRole.Equals(request.ApproverRole, StringComparison.OrdinalIgnoreCase))
                return new ApprovalResponse
                {
                    Success = false,
                    Message = $"ขั้นตอนปัจจุบันต้องการการอนุมัติจาก {currentStep.ApproverRole} ไม่ใช่ {request.ApproverRole}"
                };

            if (!stateMachine.CanTransition(currentStep.State, ApprovalState.Approved))
                return new ApprovalResponse { Success = false, Message = "ไม่สามารถอนุมัติในสถานะปัจจุบันได้" };

            var previousState = currentStep.State;
            currentStep.State = ApprovalState.Approved;
            currentStep.ApprovedBy = request.ApprovedBy;
            currentStep.ApprovedAt = DateTime.UtcNow;

            // ถ้าเป็นขั้นตอนสุดท้าย (Finance) → ส่งข้อมูลไปยัง ERP
            var isLastStep = currentStep.StepOrder == entity.Steps.Max(s => s.StepOrder);
            if (isLastStep)
            {
                var erpSuccess = erpClient.SendApprovalToERP(requestId, currentStep.ApproverRole);
                if (erpSuccess)
                {
                    currentStep.State = ApprovalState.SentToExternalSystem;
                    currentStep.IsSentToExternalSystem = true;
                    entity.OverallState = ApprovalState.SentToExternalSystem;
                }
                else
                {
                    entity.OverallState = ApprovalState.Approved;
                }
            }

            historyRepo.Add(new ApprovalHistory
            {
                RequestId = requestId,
                StepId = currentStep.Id,
                Action = "Approve",
                PerformedBy = request.ApprovedBy,
                PreviousState = previousState,
                NewState = currentStep.State
            });

            return new ApprovalResponse
            {
                Success = true,
                Message = $"{currentStep.ApproverRole} อนุมัติสำเร็จ" + (currentStep.IsSentToExternalSystem ? " (ส่งข้อมูลไปยัง ERP แล้ว)" : ""),
                Data = MapToDetail(entity)
            };
        }

        public ApprovalResponse UndoApproval(Guid requestId, UndoApproveRequest request)
        {
            var entity = approvalRepo.GetById(requestId);
            if (entity is null)
                return new ApprovalResponse { Success = false, Message = "ไม่พบคำขออนุมัติ" };

            // หา step ที่จะ undo: ใช้ StepId ที่ระบุ หรือ step ล่าสุดที่ approve
            ApprovalStep? targetStep;
            if (request.StepId.HasValue)
            {
                targetStep = entity.Steps.FirstOrDefault(s => s.Id == request.StepId.Value);
                if (targetStep is null)
                    return new ApprovalResponse { Success = false, Message = "ไม่พบขั้นตอนที่ระบุ" };
            }
            else
            {
                targetStep = stateMachine.GetLastApprovedStep(entity.Steps);
                if (targetStep is null)
                    return new ApprovalResponse { Success = false, Message = "ไม่มีขั้นตอนที่อนุมัติแล้วให้ยกเลิก" };
            }

            if (!stateMachine.CanUndo(targetStep, entity.Steps))
                return new ApprovalResponse
                {
                    Success = false,
                    Message = $"ไม่สามารถยกเลิกขั้นตอน {targetStep.ApproverRole} ได้ (มีขั้นตอนถัดไปที่อนุมัติแล้ว ต้องยกเลิกขั้นตอนล่าสุดก่อน)"
                };

            var previousState = targetStep.State;
            bool? erpRollbackRequired = null;
            bool? erpRollbackSuccess = null;

            // ถ้าข้อมูลถูกส่งไป ERP แล้ว → ต้อง rollback
            if (targetStep.IsSentToExternalSystem)
            {
                erpRollbackRequired = true;
                erpRollbackSuccess = erpClient.RollbackFromERP(requestId, targetStep.ApproverRole);

                if (erpRollbackSuccess != true)
                {
                    return new ApprovalResponse
                    {
                        Success = false,
                        Message = $"ไม่สามารถยกเลิกข้อมูลใน ERP ได้ กรุณาติดต่อทีม IT Support"
                    };
                }

                targetStep.IsSentToExternalSystem = false;
            }

            targetStep.State = ApprovalState.Undone;
            targetStep.UndoneAt = DateTime.UtcNow;

            // อัพเดท overall state
            var lastApproved = stateMachine.GetLastApprovedStep(entity.Steps);
            entity.OverallState = lastApproved is null ? ApprovalState.Pending : lastApproved.State;

            historyRepo.Add(new ApprovalHistory
            {
                RequestId = requestId,
                StepId = targetStep.Id,
                Action = "Undo",
                PerformedBy = request.RequestedBy,
                Reason = request.Reason,
                PreviousState = previousState,
                NewState = ApprovalState.Undone,
                ExternalSystemRollbackRequired = erpRollbackRequired,
                ExternalSystemRollbackSuccess = erpRollbackSuccess
            });

            var message = $"ยกเลิกการอนุมัติขั้นตอน {targetStep.ApproverRole} สำเร็จ";
            if (erpRollbackRequired == true)
                message += " (ยกเลิกข้อมูลใน ERP เรียบร้อย)";

            return new ApprovalResponse
            {
                Success = true,
                Message = message,
                Data = MapToDetail(entity)
            };
        }

        public ApprovalResponse GetById(Guid requestId)
        {
            var entity = approvalRepo.GetById(requestId);
            if (entity is null)
                return new ApprovalResponse { Success = false, Message = "ไม่พบคำขออนุมัติ" };

            return new ApprovalResponse
            {
                Success = true,
                Data = MapToDetail(entity)
            };
        }

        public List<ApprovalDetailResponse> GetAll()
        {
            return approvalRepo.GetAll().Select(MapToDetail).ToList();
        }

        public List<HistoryResponse> GetHistory(Guid requestId)
        {
            return historyRepo.GetByRequestId(requestId).Select(h => new HistoryResponse
            {
                Id = h.Id,
                StepId = h.StepId,
                Action = h.Action,
                PerformedBy = h.PerformedBy,
                PerformedAt = h.PerformedAt,
                Reason = h.Reason,
                PreviousState = h.PreviousState.ToString(),
                NewState = h.NewState.ToString(),
                ExternalSystemRollbackRequired = h.ExternalSystemRollbackRequired,
                ExternalSystemRollbackSuccess = h.ExternalSystemRollbackSuccess
            }).ToList();
        }

        private ApprovalDetailResponse MapToDetail(ApprovalRequest entity)
        {
            return new ApprovalDetailResponse
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                RequestedBy = entity.RequestedBy,
                CreatedAt = entity.CreatedAt,
                OverallState = entity.OverallState.ToString(),
                Steps = entity.Steps.OrderBy(s => s.StepOrder).Select(s => new StepDetailResponse
                {
                    Id = s.Id,
                    StepOrder = s.StepOrder,
                    ApproverRole = s.ApproverRole,
                    State = s.State.ToString(),
                    ApprovedBy = s.ApprovedBy,
                    ApprovedAt = s.ApprovedAt,
                    IsSentToExternalSystem = s.IsSentToExternalSystem,
                    CanUndo = stateMachine.CanUndo(s, entity.Steps)
                }).ToList()
            };
        }
    }
}
