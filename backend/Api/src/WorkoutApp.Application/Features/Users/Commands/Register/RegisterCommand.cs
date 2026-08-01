using MediatR;
using WorkoutApp.Application.DTOs.User.RegisterUser;
using WorkoutApp.Domain.Common;

namespace WorkoutApp.Application.Features.Users.Commands.Register;

public sealed record RegisterCommand(RegisterRequest Request)
    : IRequest<Result<RegisterResponse>>;