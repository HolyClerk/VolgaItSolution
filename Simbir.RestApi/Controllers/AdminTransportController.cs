using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simbir.Application.Abstractions;
using Simbir.Core.Requests;

namespace Simbir.RestApi.Controllers;

[ApiController]
[Route("api/Admin/Transport")]
public class AdminTransportController : ControllerBase
{
    private readonly ITransportService _transportService;
    private readonly IAccountService _accountService;

    public AdminTransportController(ITransportService transportService, 
        IAccountService accountService)
    {
        _transportService = transportService;
        _accountService = accountService;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult> GetTransportInRange(int start, int count, string transportType)
    {
        if (!await _accountService.IsAdministrator(User))
            return Unauthorized();

        var result = await _transportService.GetInRange(start, count);
        return Ok(result.Where(x => x.TransportType == transportType));
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult> GetTransport(int id)
    {
        if (!await _accountService.IsAdministrator(User))
            return Unauthorized();

        var result = await _transportService.GetAsync(id);

        return result.Succeeded switch
        {
            true => Ok(result.Value),
            false => BadRequest(result.Errors)
        };
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> AddTransport([FromBody] ForceAddTransportRequest request)
    {
        if (!await _accountService.IsAdministrator(User))
            return Unauthorized();

        var result = await _transportService.AddAsync(request);

        return result.Succeeded switch
        {
            true => Ok(),
            false => BadRequest(result.Errors)
        };
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateTransport([FromBody] ForceUpdateTransportRequest request, int id)
    {
        if (!await _accountService.IsAdministrator(User))
            return Unauthorized();

        var result = await _transportService.ForceUpdateAsync(id, request);

        return result.Succeeded switch
        {
            true => Ok(),
            false => BadRequest(result.Errors)
        };
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> RemoveTransport(int id)
    {
        if (!await _accountService.IsAdministrator(User))
            return Unauthorized();

        var result = await _transportService.ForceRemoveAsync(id);

        return result.Succeeded switch
        {
            true => Ok(),
            false => BadRequest(result.Errors)
        };
    }
}
