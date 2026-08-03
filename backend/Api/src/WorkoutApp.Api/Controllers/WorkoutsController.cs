using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkoutApp.Api.Common;
using WorkoutApp.Application.DTOs.Workout.LogWorkout;
using WorkoutApp.Application.Features.Workouts.Commands.LogWorkout;
using WorkoutApp.Application.Features.Workouts.Queries.GetMonthlyProgress;
using WorkoutApp.Application.Features.Workouts.Queries.GetWorkouts;

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

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetWorkoutsQuery(), cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("progress")]
    public async Task<IActionResult> GetMonthlyProgress([FromQuery] int year, [FromQuery] int month, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetMonthlyProgressQuery(year, month), cancellationToken);
        return result.ToActionResult();
    }
}