using System.Security.Claims;
using Simbir.Core.Entities;
using Simbir.Core.Requests;
using Simbir.Core.Results;

namespace Simbir.Application.Abstractions;

public interface IRentService
{
    Result<List<Transport>> GetRentableTransport(GetRentableRequest request);

    Task<Result<Rent>> GetAsync(long rentId, ClaimsPrincipal claims);
    Task<Result<Rent>> ForceGetAsync(long rentId);

    Task<Result<List<Rent>>> GetAccountHistoryAsync(ClaimsPrincipal claims);
    Task<List<Rent>> ForceGetAccountHistoryAsync(long userId);

    Task<Result<List<Rent>>> GetTransportHistoryAsync(long transportId, ClaimsPrincipal claims);
    Task<List<Rent>> ForceGetTransportHistoryAsync(long transportId);

    Task<Result> ForceUpdateAsync(long rentId, ForceUpdateRentRequest request);

    Task<Result> RentAsync(long transportId, string rentType, ClaimsPrincipal renterClaims);
    Task<Result> EndRentAsync(long rentId, EndRentRequest request, ClaimsPrincipal renterClaims);
    Task<Result> ForceEndRentAsync(long rentId, EndRentRequest request);

    Task<Result> ForceRemoveRentAsync(long rentId);
}
