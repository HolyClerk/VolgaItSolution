using System.Security.Claims;
using Simbir.Core.Entities;
using Simbir.Core.Requests;
using Simbir.Core.Results;

namespace Simbir.Application.Abstractions;

public interface ITransportService
{
    Task<Result<Transport>> GetTransportAsync(int id);
    Task<Result> AddTransportAsync(AddTransportRequest request, ClaimsPrincipal claims);
    Task<Result> UpdateTransportAsync(int transportId, UpdateTransportRequest request, ClaimsPrincipal claims);
    Task<Result> RemoveTransportAsync(int id, ClaimsPrincipal claims);
}
