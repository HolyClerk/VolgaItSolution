using System.ComponentModel.DataAnnotations;

namespace Simbir.Core.AccountRequests;

public class SignInRequest
{
    [MinLength(6), MaxLength(40)]
    public string Username { get; set; }

    [MinLength(6), MaxLength(40)]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}

public class SignUpRequest : SignInRequest { }
public class UpdateRequest : SignInRequest 
{
    public string NewPassword { get; set; }
}