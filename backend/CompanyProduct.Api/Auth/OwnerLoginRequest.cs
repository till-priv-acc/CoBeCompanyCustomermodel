namespace CompanyProduct.Api.Auth;

public class OwnerLoginRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}