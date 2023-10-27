using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Simbir.Application.Other;
using Simbir.Core.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Simbir.Infrastructure.Implementations;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public const int TokenLifetime = 60 * 2;

    public const string Issuer = "https://localhost:7125";
    public const string Audience = "https://localhost:7125";

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var generator = RandomNumberGenerator.Create();
        generator.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public string CreateToken(ApplicationUser user)
    {
        var claims = new List<Claim>()
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName!),
        };

        var securityKey = GetSymmetricSecurityKey();
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            notBefore: DateTime.Now,
            expires: DateTime.Now.AddMinutes(TokenLifetime),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        byte[] key = Encoding.ASCII.GetBytes(_configuration["SecretKey"]!);
        return new SymmetricSecurityKey(key);
    }

    public ClaimsPrincipal? GetClaimsPrincipal(string token)
    {
        ClaimsPrincipal claimsPrincipal;
        SecurityToken securityToken;

        var tokenHandler = new JwtSecurityTokenHandler();
        var parameters = ParametersHelper.GetTokenExpireParameters(_configuration);

        try
        {
            claimsPrincipal = tokenHandler.ValidateToken(token, parameters, out securityToken);
        }
        catch (Exception)
        {
            return null;
        }

        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !IsAlgorithmsEquals(jwtSecurityToken.Header.Alg))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return claimsPrincipal;
    }

    private static bool IsAlgorithmsEquals(string algorithm)
        => algorithm.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
}