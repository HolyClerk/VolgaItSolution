using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Simbir.Application.Abstractions;
using Simbir.Core.Entities;

namespace Simbir.RestApi.Controllers;

[ApiController]
[Route("api/Payment")]
public class PaymentController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly UserManager<ApplicationUser> _userManager;

    public PaymentController(IAccountService accountService,
        UserManager<ApplicationUser> userManager)
    {
        _accountService = accountService;
        _userManager = userManager;
    }

    [Authorize]
    [HttpPost("Hesoyam/{accountId}")]
    public async Task<ActionResult> AddMoneyToBalance(int accountId)
    {
        var currentUser = await _accountService.GetUserByClaimsAsync(User);
        var user = await _userManager.FindByIdAsync(accountId.ToString());

        if (currentUser is null || user is null)
            return BadRequest();

        if (currentUser.Id == user.Id)
        {
            currentUser.Balance += 250_000;
            await _userManager.UpdateAsync(currentUser);
            return Ok();
        }

        if (currentUser.IsAdministrator)
        {
            user.Balance += 250_000;
            await _userManager.UpdateAsync(user);
            return Ok();
        }

        return Unauthorized();
    }
}
