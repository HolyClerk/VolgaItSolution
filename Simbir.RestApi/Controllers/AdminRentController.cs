using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simbir.Application.Abstractions;
using Simbir.Core.Requests;

namespace Simbir.RestApi.Controllers;

[ApiController]
[Route("api/Admin")]
public class AdminRentController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IRentService _rentService;

    public AdminRentController(IAccountService accountService, 
        IRentService rentService)
    {
        _accountService = accountService;
        _rentService = rentService;
    }

    [Authorize]
    [HttpGet("Rent/{rentId}")]
    public async Task<ActionResult> GetRent(long rentId)
    {
        if (!await _accountService.IsAdministrator(User))
            return Unauthorized();

        var result = await _rentService.ForceGetAsync(rentId);

        return result.Succeeded switch
        {
            true => Ok(result.Value),
            false => BadRequest(result.Errors)
        };
    }

    [Authorize]
    [HttpGet("UserHistory/{userId}")]
    public async Task<ActionResult> GetRentAccountHistory(long userId)
    {
        if (!await _accountService.IsAdministrator(User))
            return Unauthorized();

        var result = await _rentService.ForceGetAccountHistoryAsync(userId);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("TransportHistory/{transportId}")]
    public async Task<ActionResult> GetRentTransportHistory(long transportId)
    {
        if (!await _accountService.IsAdministrator(User))
            return Unauthorized();

        var result = await _rentService.ForceGetTransportHistoryAsync(transportId);

        return Ok(result);
    }

    [Authorize]
    [HttpPost("Rent")]
    public async Task<ActionResult> CreateRent([FromBody] ForceRentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _accountService.IsAdministrator(User))
            return Unauthorized();

        var result = await _rentService.ForceRentAsync(request);

        return Ok(result);
    }

    [Authorize]
    [HttpPost("Rent/End/{rentId}")]
    public async Task<ActionResult> EndRent(long rentId, [FromBody] EndRentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _accountService.IsAdministrator(User))
            return Unauthorized();

        var result = await _rentService.ForceEndRentAsync(rentId, request);

        return Ok(result);
    }

    [Authorize]
    [HttpPut("Rent/{rentId}")]
    public async Task<ActionResult> UpdateRent(long rentId, [FromBody] ForceRentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _accountService.IsAdministrator(User))
            return Unauthorized();

        var result = await _rentService.ForceUpdateAsync(rentId, request);

        return Ok(result);
    }

    [Authorize]
    [HttpDelete("Rent/{rentId}")]
    public async Task<ActionResult> RemoveRent(long rentId)
    {
        if (!await _accountService.IsAdministrator(User))
            return Unauthorized();

        var result = await _rentService.ForceRemoveRentAsync(rentId);

        return Ok(result);
    }
}
