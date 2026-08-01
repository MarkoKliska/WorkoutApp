using MediatR;
using WorkoutApp.Application.DTOs.User.LoginUser;
using WorkoutApp.Domain.Common;

namespace WorkoutApp.Application.Features.Users.Queries.Login;

public sealed record LoginQuery(LoginRequest Request) : IRequest<Result<LoginResponse>>;