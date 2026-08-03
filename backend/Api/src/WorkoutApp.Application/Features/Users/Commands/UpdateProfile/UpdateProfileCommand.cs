using MediatR;
using WorkoutApp.Application.DTOs.User.GetProfile;
using WorkoutApp.Application.DTOs.User.UpdateProfile;
using WorkoutApp.Domain.Common;

namespace WorkoutApp.Application.Features.Users.Commands.UpdateProfile;

public sealed record UpdateProfileCommand(UpdateProfileRequest Request) 
    : IRequest<Result<ProfileResponse>>;