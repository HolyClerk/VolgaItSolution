using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simbir.Application.Abstractions;
using Simbir.Core.Requests;

namespace Simbir.RestApi.Controllers;

[ApiController]
[Route("api/Rent")]
public class RentController : ControllerBase
{
    private readonly IRentService _rentService;

    public RentController(IRentService rentService)
    {
        _rentService = rentService;
    }

    [HttpGet("Transport")]
    public ActionResult GetRentableTransport(double latitude, double longitude, double radius, string type)
    {
        var request = new GetRentableRequest(latitude, longitude, radius, type);
        var result = _rentService.GetRentableTransport(request);

        return result.Succeeded switch
        {
            true => Ok(result),
            false => BadRequest(result.Errors)
        };
    }

    [Authorize]
    [HttpGet("{rentId}")]
    public async Task<ActionResult> GetRentInfo(long rentId)
    {
        var result = await _rentService.GetAsync(rentId, User);

        return result.Succeeded switch
        {
            true => Ok(result),
            false => BadRequest(result.Errors)
        };
    }

    [Authorize]
    [HttpGet("MyHistory")]
    public async Task<ActionResult> GetRentHistory()
    {
        var result = await _rentService.GetAccountHistoryAsync(User);

        return result.Succeeded switch
        {
            true => Ok(result),
            false => BadRequest(result.Errors)
        };
    }

    [Authorize]
    [HttpGet("TransportHistory/{transportId}")]
    public async Task<ActionResult> GetRentHistory(long transportId)
    {
        var result = await _rentService.GetTransportHistoryAsync(transportId, User);

        return result.Succeeded switch
        {
            true => Ok(result),
            false => BadRequest(result.Errors)
        };
    }

    [Authorize]
    [HttpPost("New/{tranposrtId}")]
    public async Task<ActionResult> StartRent(long tranposrtId, [FromBody] StartRentRequest request)
    {
        var result = await _rentService.RentAsync(tranposrtId, request.RentType, User);

        return result.Succeeded switch
        {
            true => Ok(result),
            false => BadRequest(result.Errors)
        };
    }

    [Authorize]
    [HttpPost("End/{rentId}")]
    public async Task<ActionResult> EndRent(long rentId, [FromBody] EndRentRequest request)
    {
        var result = await _rentService.EndRentAsync(rentId, request, User);

        return result.Succeeded switch
        {
            true => Ok(result),
            false => BadRequest(result.Errors)
        };
    }
}
