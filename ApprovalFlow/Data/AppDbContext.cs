using ApprovalFlow.Workflow;

namespace ApprovalFlow.Data
{
    public class AppDbContext
    {
        public List<ApprovalRequest> ApprovalRequests { get; } = [];
        public List<ApprovalHistory> ApprovalHistories { get; } = [];

        public AppDbContext()
        {
            SeedData();
        }

        private void SeedData()
        {
            var requestId1 = Guid.Parse("a1b2c3d4-0001-0000-0000-000000000001");
            var requestId2 = Guid.Parse("a1b2c3d4-0002-0000-0000-000000000002");
            var requestId3 = Guid.Parse("a1b2c3d4-0003-0000-0000-000000000003");

            // คำขอที่ 1: ยังไม่มีใครอนุมัติ
            ApprovalRequests.Add(new ApprovalRequest
            {
                Id = requestId1,
                Title = "ขออนุมัติจัดซื้ออุปกรณ์ IT",
                Description = "ขอจัดซื้อ Laptop 10 เครื่อง สำหรับทีมพัฒนา งบประมาณ 500,000 บาท",
                RequestedBy = "สมชาย",
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                OverallState = ApprovalState.Pending,
                Steps =
                [
                    new() { Id = Guid.Parse("b0000001-0001-0000-0000-000000000001"), RequestId = requestId1, StepOrder = 1, ApproverRole = "Manager" },
                    new() { Id = Guid.Parse("b0000001-0002-0000-0000-000000000002"), RequestId = requestId1, StepOrder = 2, ApproverRole = "Director" },
                    new() { Id = Guid.Parse("b0000001-0003-0000-0000-000000000003"), RequestId = requestId1, StepOrder = 3, ApproverRole = "Finance" },
                ]
            });

            // คำขอที่ 2: Manager อนุมัติแล้ว รอ Director
            ApprovalRequests.Add(new ApprovalRequest
            {
                Id = requestId2,
                Title = "ขออนุมัติงบฝึกอบรม",
                Description = "อบรมหลักสูตร Cloud Architecture 3 วัน งบประมาณ 45,000 บาท",
                RequestedBy = "สมหญิง",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                OverallState = ApprovalState.Pending,
                Steps =
                [
                    new() { Id = Guid.Parse("b0000002-0001-0000-0000-000000000001"), RequestId = requestId2, StepOrder = 1, ApproverRole = "Manager", State = ApprovalState.Approved, ApprovedBy = "วิชัย (Manager)", ApprovedAt = DateTime.UtcNow.AddDays(-4) },
                    new() { Id = Guid.Parse("b0000002-0002-0000-0000-000000000002"), RequestId = requestId2, StepOrder = 2, ApproverRole = "Director" },
                    new() { Id = Guid.Parse("b0000002-0003-0000-0000-000000000003"), RequestId = requestId2, StepOrder = 3, ApproverRole = "Finance" },
                ]
            });

            // คำขอที่ 3: อนุมัติครบทุกขั้นตอน + ส่งข้อมูลไป ERP แล้ว
            ApprovalRequests.Add(new ApprovalRequest
            {
                Id = requestId3,
                Title = "ขออนุมัติจัดจ้าง Vendor",
                Description = "จัดจ้าง Vendor พัฒนาระบบ Mobile App งบประมาณ 2,000,000 บาท",
                RequestedBy = "วิภา",
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                OverallState = ApprovalState.SentToExternalSystem,
                Steps =
                [
                    new() { Id = Guid.Parse("b0000003-0001-0000-0000-000000000001"), RequestId = requestId3, StepOrder = 1, ApproverRole = "Manager", State = ApprovalState.Approved, ApprovedBy = "วิชัย (Manager)", ApprovedAt = DateTime.UtcNow.AddDays(-9) },
                    new() { Id = Guid.Parse("b0000003-0002-0000-0000-000000000002"), RequestId = requestId3, StepOrder = 2, ApproverRole = "Director", State = ApprovalState.Approved, ApprovedBy = "ประยุทธ์ (Director)", ApprovedAt = DateTime.UtcNow.AddDays(-7) },
                    new() { Id = Guid.Parse("b0000003-0003-0000-0000-000000000003"), RequestId = requestId3, StepOrder = 3, ApproverRole = "Finance", State = ApprovalState.SentToExternalSystem, ApprovedBy = "มาลี (Finance)", ApprovedAt = DateTime.UtcNow.AddDays(-5), IsSentToExternalSystem = true },
                ]
            });

            // เพิ่ม History สำหรับคำขอที่ 2 และ 3
            ApprovalHistories.Add(new ApprovalHistory
            {
                RequestId = requestId2,
                StepId = Guid.Parse("b0000002-0001-0000-0000-000000000001"),
                Action = "Approve",
                PerformedBy = "วิชัย (Manager)",
                PerformedAt = DateTime.UtcNow.AddDays(-4),
                PreviousState = ApprovalState.Pending,
                NewState = ApprovalState.Approved
            });

            ApprovalHistories.Add(new ApprovalHistory
            {
                RequestId = requestId3,
                StepId = Guid.Parse("b0000003-0001-0000-0000-000000000001"),
                Action = "Approve",
                PerformedBy = "วิชัย (Manager)",
                PerformedAt = DateTime.UtcNow.AddDays(-9),
                PreviousState = ApprovalState.Pending,
                NewState = ApprovalState.Approved
            });

            ApprovalHistories.Add(new ApprovalHistory
            {
                RequestId = requestId3,
                StepId = Guid.Parse("b0000003-0002-0000-0000-000000000002"),
                Action = "Approve",
                PerformedBy = "ประยุทธ์ (Director)",
                PerformedAt = DateTime.UtcNow.AddDays(-7),
                PreviousState = ApprovalState.Pending,
                NewState = ApprovalState.Approved
            });

            ApprovalHistories.Add(new ApprovalHistory
            {
                RequestId = requestId3,
                StepId = Guid.Parse("b0000003-0003-0000-0000-000000000003"),
                Action = "Approve",
                PerformedBy = "มาคะบุชิ (Finance)",
                PerformedAt = DateTime.UtcNow.AddDays(-5),
                PreviousState = ApprovalState.Pending,
                NewState = ApprovalState.SentToExternalSystem,
                ExternalSystemRollbackRequired = false
            });
        }
    }
}
