using Simbir.Core.DTO;
using Simbir.Core.AccountRequests;
using Simbir.Core.Results;
using System.Security.Claims;
using Simbir.Core.Entities;

namespace Simbir.Application.Abstractions;

public interface IAccountService
{
    Task<Result<Credentials>> SignInAsync(SignInRequest request);
     
    // SingUp не возвращает Credentials т.к. клиент после этого должен самостоятельно залогиниться, чтобы получить токен
    Task<Result> SignUpAsync(SignUpRequest request);
    Task<Result> ForceCreateAccountAsync(ForceAccountRequest request);

    Task<Result> UpdateAsync(UpdateRequest request, ClaimsPrincipal userClaims);
    Task<Result> ForceUpdateAsync(long id, ForceAccountRequest request);

    Task<List<ApplicationUser>> GetInRangeAsync(int start, int count);
    Task<ApplicationUser?> GetUserByClaimsAsync(ClaimsPrincipal? userClaims);
    Task<ApplicationUser?> GetUserByIdAsync(long id);

    Task<Result> ForceRemoveAccount(long id);
    Task<bool> IsAdministrator(ClaimsPrincipal? userClaims);
}