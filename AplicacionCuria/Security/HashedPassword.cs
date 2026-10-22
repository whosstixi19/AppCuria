namespace AplicacionCuria.Security;

public class HashedPassword
{
    public string? Password { get; set; }
    public string? Salt { get; set; }
    public int Iterations { get; set; }
}
