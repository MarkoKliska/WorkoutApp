using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkoutApp.Api.Common;
using WorkoutApp.Application.DTOs.Workout.LogWorkout;
using WorkoutApp.Application.Features.Workouts.Commands.LogWorkout;

namespace WorkoutApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class WorkoutsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Log(LogWorkoutRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new LogWorkoutCommand(request), cancellationToken);
        return result.ToActionResult();
    }
}