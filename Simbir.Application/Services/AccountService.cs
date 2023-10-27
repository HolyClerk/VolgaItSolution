using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Simbir.Application.Abstractions;
using Simbir.Application.Other;
using Simbir.Core.DTO;
using Simbir.Core.Entities;
using Simbir.Core.AccountRequests;
using Simbir.Core.Results;

namespace Simbir.Application.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IDbContext _context;
    private readonly ITokenService _tokenService;

    public AccountService(UserManager<ApplicationUser> userManager,
        IDbContext context,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<Result<Credentials>> SignInAsync(SignInRequest request)
    {
        var validationResult = await ValidateSignInAsync(request);

        if (!validationResult.Succeeded)
            return Result<Credentials>.Failed(validationResult.Errors);

        var credentials = CreateToken(validationResult.Value);
        return Result<Credentials>.Success(credentials);
    }

    public async Task<Result> SignUpAsync(SignUpRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Username,
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return Result.Failed(result.Errors.Select(x => x.Description).ToArray());

        return Result.Success();
    }

    public async Task<Result> UpdateAsync(UpdateRequest request, ClaimsPrincipal claims)
    {
        var applicationUser = await GetUserByClaimsAsync(claims);

        if (applicationUser is null)
            return Result.Failed("Непредвиденная ошибка. Claims не найдены.");

        var hashedPassword = _userManager.PasswordHasher.HashPassword(applicationUser, request.Password);

        applicationUser.UserName = request.Username;
        applicationUser.PasswordHash = hashedPassword;

        var identityResult = await _userManager.UpdateAsync(applicationUser);

        if (!identityResult.Succeeded)
            return Result.Failed(identityResult.Errors.Select(x => x.Description).ToArray());

        return Result.Success();
    }

    private Credentials CreateToken(ApplicationUser user)
    {
        var accessToken = _tokenService.CreateToken(user);

        return new Credentials
        {
            Token = accessToken
        };
    }

    private async Task<Result<ApplicationUser>> ValidateSignInAsync(SignInRequest request)
    {
        var managedUser = await _userManager.FindByNameAsync(request.Username);

        if (managedUser is null)
            return Result<ApplicationUser>.Failed("Некорректные данные для входа");

        var isPasswordValid = await _userManager.CheckPasswordAsync(managedUser, request.Password);

        if (!isPasswordValid)
            return Result<ApplicationUser>.Failed("Некорректные данные для входа");

        var user = _context.Users.SingleOrDefault(user => user.UserName == request.Username);

        if (user is null)
            return Result<ApplicationUser>.Failed("Некорректные данные для входа");

        return Result<ApplicationUser>.Success(user);
    }

    private async Task<ApplicationUser?> GetUserByClaimsAsync(ClaimsPrincipal? principal)
    {
        if (principal is null || principal.Identity is null)
            return null;

        var idClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
        return await _userManager.FindByIdAsync(idClaim!.Value);
    }
}
