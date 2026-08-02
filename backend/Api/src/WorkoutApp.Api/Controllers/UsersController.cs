using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkoutApp.Api.Common;
using WorkoutApp.Application.DTOs.User.UpdateProfile;
using WorkoutApp.Application.Features.Users.Commands.UpdateProfile;
using WorkoutApp.Application.Features.Users.Queries.GetProfile;

namespace WorkoutApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class UsersController(ISender sender) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetProfileQuery(), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateProfileCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}