using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkoutApp.Api.Common;
using WorkoutApp.Application.DTOs.User.LoginUser;
using WorkoutApp.Application.DTOs.User.RegisterUser;
using WorkoutApp.Application.Features.Users.Commands.Register;
using WorkoutApp.Application.Features.Users.Queries.Login;

namespace WorkoutApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RegisterCommand(request), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new LoginQuery(request), cancellationToken);
        return result.ToActionResult();
    }
}