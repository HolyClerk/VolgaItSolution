using Microsoft.IdentityModel.Tokens;
using Simbir.Core.Entities;
using System.Security.Claims;

namespace Simbir.Application.Other;

public interface ITokenService
{
    string CreateToken(ApplicationUser user);
    SymmetricSecurityKey GetSymmetricSecurityKey();
    ClaimsPrincipal? GetClaimsPrincipal(string token);
    string CreateRefreshToken();
}
