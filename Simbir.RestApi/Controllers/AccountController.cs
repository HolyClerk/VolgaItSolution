using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simbir.Application.Abstractions;
using Simbir.Core.AccountRequests;

namespace Simbir.RestApi.Controllers;

[ApiController]
[Route("api/Account")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet("Me")]
    public async Task<ActionResult> GetAccountInfo()
    {
        var result = await _accountService.SignInAsync(request);

        return result.Succeeded switch
        {
            true => Ok(result.Succeeded),
            false => BadRequest(result.Errors)
        };
    }

    [HttpPost("SignIn")]
    public async Task<ActionResult> SignIn([FromBody] SignInRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _accountService.SignInAsync(request);

        return result.Succeeded switch
        {
            true => Ok(result.Succeeded),
            false => BadRequest(result.Errors)
        };
    }

    [HttpPost("SignUp")]
    public async Task<ActionResult> SignUp([FromBody] SignUpRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _accountService.SignUpAsync(request);

        return result.Succeeded switch
        {
            true => Ok(result.Succeeded),
            false => BadRequest(result.Errors)
        };
    }

    [Authorize]
    [HttpPut("Update")]
    public async Task<ActionResult> Update([FromBody] UpdateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _accountService.UpdateAsync(request, User);

        return result.Succeeded switch
        {
            true => Ok(result.Succeeded),
            false => BadRequest(result.Errors)
        };
    }

    /*    
        private ActionResult HandleResult<T>(Result<T> result)
        {
            return result.Succeeded switch
            {
                true => Ok(result.Succeeded),
                false => BadRequest(result.Errors)
            };
        }*/
}
