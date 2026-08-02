using MediatR;
using WorkoutApp.Application.DTOs.User.GetProfile;
using WorkoutApp.Domain.Common;

namespace WorkoutApp.Application.Features.Users.Queries.GetProfile;

public sealed record GetProfileQuery 
    : IRequest<Result<ProfileResponse>>;