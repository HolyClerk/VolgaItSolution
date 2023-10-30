using System.ComponentModel.DataAnnotations;

namespace Simbir.Core.AccountRequests;

public class SignInRequest
{
    [MinLength(4), MaxLength(40)]
    public string Username { get; set; }

    [MinLength(6), MaxLength(40)]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}

public class SignUpRequest : SignInRequest { }
public record ForceAccountRequest (string Username, string Password, bool IsAdministrator, float Balance);
public class UpdateRequest : SignInRequest { }