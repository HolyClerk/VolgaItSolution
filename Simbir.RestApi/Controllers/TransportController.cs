using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simbir.Application.Services;
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
    public async Task<ActionResult> GetTransport(int id)
    {
        var result = await _transportService.GetTransportAsync(id);

        return result.Succeeded switch
        {
            true => Ok(result.Succeeded),
            false => BadRequest(result.Errors)
        };
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> AddTransport(AddTransportRequest request)
    {
        var result = await _transportService.AddTransportAsync(request, User);

        return result.Succeeded switch
        {
            true => Ok(result.Succeeded),
            false => BadRequest(result.Errors)
        };
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateTransport(int id, UpdateTransportRequest request)
    {
        var result = await _transportService.UpdateTransportAsync(id, request, User);

        return result.Succeeded switch
        {
            true => Ok(result.Succeeded),
            false => BadRequest(result.Errors)
        };
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> RemoveTransport(int id)
    {
        var result = await _transportService.RemoveTransportAsync(id, User);

        return result.Succeeded switch
        {
            true => Ok(result.Succeeded),
            false => BadRequest(result.Errors)
        };
    }
}
