using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simbir.Application.Abstractions;
using Simbir.Core.AccountRequests;
using Simbir.Core.Entities;

namespace Simbir.RestApi.Controllers;


[ApiController]
[Route("api/Admin/Account")]
public class AdminAccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AdminAccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult> GetAccountsInRange(int start, int count)
    {
        if (!await _accountService.IsAdministrator(User))
            return Unauthorized();

        var accounts = await _accountService.GetInRangeAsync(start, count);
        return Ok(accounts);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult> GetAccountInfo(long id)
    {
        if (!await _accountService.IsAdministrator(User))
            return Unauthorized();

        var result = await _accountService.GetUserByIdAsync(id);

        return result switch
        {
            ApplicationUser => Ok(result),
            null => BadRequest()
        };
    }

    // Читать Readme.md
    [HttpPost]
    public async Task<ActionResult> CreateAccount([FromBody] ForceAccountRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _accountService.ForceCreateAccountAsync(request);

        return result.Succeeded switch
        {
            true => Ok(result),
            false => BadRequest()
        };
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAccount(long id, [FromBody] ForceAccountRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _accountService.IsAdministrator(User))
            return Unauthorized();

        var result = await _accountService.ForceUpdateAsync(id, request);

        return result.Succeeded switch
        {
            true => Ok(result),
            false => BadRequest()
        };
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> RemoveAccount(long id)
    {
        if (!await _accountService.IsAdministrator(User))
            return Unauthorized();

        var result = await _accountService.ForceRemoveAccount(id);

        return result.Succeeded switch
        {
            true => Ok(result),
            false => BadRequest()
        };
    }
}
