using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simbir.Application.Abstractions;
using Simbir.Core.Requests;

namespace Simbir.RestApi.Controllers;

[ApiController]
[Route("api/Transport")]
public class TransportController : ControllerBase
{
    private readonly ITransportService _transportService;

    public TransportController(ITransportService transportService)
    {
        _transportService = transportService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetTransport(long id)
    {
        var result = await _transportService.GetAsync(id);

        return result.Succeeded switch
        {
            true => Ok(result.Value),
            false => BadRequest(result.Errors)
        };
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> AddTransport(AddTransportRequest request)
    {
        var result = await _transportService.AddAsync(request, User);

        return result.Succeeded switch
        {
            true => Ok(),
            false => BadRequest(result.Errors)
        };
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateTransport(long id, UpdateTransportRequest request)
    {
        var result = await _transportService.UpdateAsync(id, request, User);

        return result.Succeeded switch
        {
            true => Ok(),
            false => BadRequest(result.Errors)
        };
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> RemoveTransport(long id)
    {
        var result = await _transportService.RemoveAsync(id, User);

        return result.Succeeded switch
        {
            true => Ok(),
            false => BadRequest(result.Errors)
        };
    }
}
