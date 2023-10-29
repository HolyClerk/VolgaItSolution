using Microsoft.AspNetCore.Mvc;
using Simbir.Application.Abstractions;

namespace Simbir.RestApi.Controllers;

[ApiController]
[Route("api/Admin/Rent")]
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
}
