namespace ApprovalFlow.Integrations
{
    public class ERPClient
    {
        public bool SendApprovalToERP(Guid requestId, string stepRole)
        {
            Console.WriteLine($"[ERP] ส่งข้อมูลอนุมัติไปยัง ERP: Request={requestId}, Step={stepRole}");
            return true;
        }

        public bool RollbackFromERP(Guid requestId, string stepRole)
        {
            Console.WriteLine($"[ERP] ยกเลิกข้อมูลใน ERP: Request={requestId}, Step={stepRole}");
            return true;
        }
    }
}
