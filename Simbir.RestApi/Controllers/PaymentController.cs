using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Simbir.Application.Abstractions;
using Simbir.Application.Other;
using Simbir.Core.Entities;

namespace Simbir.RestApi.Controllers;

[ApiController]
[Route("api/Payment")]
public class PaymentController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IDbContext _context;

    public PaymentController(IAccountService accountService, 
        IDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _accountService = accountService;
        _context = context;
        _userManager = userManager;
    }

    [Authorize]
    [HttpPost("Hesoyam/{accountId}")]
    public async Task<ActionResult> AddMoneyToBalance(int accountId)
    {
        var currentUser = await _accountService.GetUserByClaimsAsync(User);

        if (currentUser is null)
            return BadRequest();

        var isAdministrator = await _context.Administrators.AnyAsync(admin => admin.UserId == currentUser.Id);

        // Если себе
        if (currentUser.Id == accountId)
        {
            currentUser.Balance += 250_000;
            await _context.SaveChangesAsync();
            return Ok();
        }

        // Если юзер - админ
        if (!isAdministrator)
            return BadRequest();

        var user = await _userManager.FindByIdAsync(accountId.ToString());

        if (user is not null)
        {
            user.Balance += 250_000;
            await _userManager.UpdateAsync(user);
        }

        // if user admin add too
        return BadRequest();
    }
}
