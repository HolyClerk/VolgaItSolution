using System.Security.Claims;
using Simbir.Core.Entities;
using Simbir.Core.Requests;
using Simbir.Core.Results;

namespace Simbir.Application.Abstractions;

public interface ITransportService
{
    Task<Result<Transport>> GetAsync(long id);
    Task<Result> AddAsync(AddTransportRequest request, ClaimsPrincipal userClaims);
    Task<Result> ForceAddAsync(ForceAddTransportRequest request);
    Task<Result> UpdateAsync(long transportId, UpdateTransportRequest request, ClaimsPrincipal userClaims);
    Task<Result> ForceUpdateAsync(long transportId, ForceUpdateTransportRequest request);
    Task<Result> RemoveAsync(long id, ClaimsPrincipal userClaims);
    Task<Result> ForceRemoveAsync(long id);
    Task<List<Transport>> GetInRange(int start, int count);
}
