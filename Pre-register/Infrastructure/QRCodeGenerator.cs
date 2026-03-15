namespace Pre_register.Infrastructure;

public class QRCodeGenerator
{
    public string Generate(string token)
    {
        return $"PRE-REG:{token}";
    }
}
