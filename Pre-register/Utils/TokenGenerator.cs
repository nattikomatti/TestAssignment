namespace Pre_register.Utils;

public class TokenGenerator
{
    public string Generate()
    {
        return $"TKN-{Guid.NewGuid().ToString("N")[..4].ToUpper()}";
    }
}
