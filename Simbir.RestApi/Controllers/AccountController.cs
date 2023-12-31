﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Simbir.Application.Abstractions;
using Simbir.Core.AccountRequests;
using Simbir.Core.Entities;

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

    [Authorize]
    [HttpGet("Me")]
    public async Task<ActionResult> GetAccountInfo()
    {
        var result = await _accountService.GetUserByClaimsAsync(User);

        return result switch
        {
            ApplicationUser => Ok(result),
            null => BadRequest()
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
            true => Ok(result.Value),
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
            true => Ok(),
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
            true => Ok(),
            false => BadRequest(result.Errors)
        };
    }
}
