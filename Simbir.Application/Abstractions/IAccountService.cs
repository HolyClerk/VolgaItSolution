using Simbir.Core.DTO;
using Simbir.Core.AccountRequests;
using Simbir.Core.Results;
using System.Security.Claims;

namespace Simbir.Application.Abstractions;

public interface IAccountService
{
    Task<Result<Credentials>> SignInAsync(SignInRequest request);
     
    // SingUp не возвращает Credentials т.к. клиент после этого должен самостоятельно залогиниться, чтобы получить токен
    Task<Result> SignUpAsync(SignUpRequest request);
    Task<Result> UpdateAsync(UpdateRequest request, ClaimsPrincipal claims);

    //Task<Result<Credentials>> RefreshToken(RefreshTokenRequest request);
}