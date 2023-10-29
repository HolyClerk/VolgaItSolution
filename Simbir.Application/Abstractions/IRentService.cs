using System.Security.Claims;
using Simbir.Core.Entities;
using Simbir.Core.Requests;
using Simbir.Core.Results;

namespace Simbir.Application.Abstractions;

public interface IRentService
{
    Result<List<Transport>> GetRentableTransport(GetRentableRequest request);
    Task<Result<Rent>> GetRentAsync(int rentId, ClaimsPrincipal claims);
    Task<Result<List<Rent>>> GetAccountHistoryAsync(ClaimsPrincipal claims);
    Task<Result<List<Rent>>> GetTransportHistoryAsync(int transportId, ClaimsPrincipal claims);
    Task<Result> StartNewRentAsync(int transportId, string rentType, ClaimsPrincipal renterClaims);
    Task<Result> EndRentAsync(int rentId, EndRentRequest request, ClaimsPrincipal renterClaims);
}
