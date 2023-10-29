using Microsoft.AspNetCore.Identity;

namespace Simbir.Core.Entities;

public class ApplicationUser : IdentityUser<long>
{
    public float Balance { get; set; }
    public bool IsAdministrator { get; set; }
}