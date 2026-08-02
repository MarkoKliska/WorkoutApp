using MediatR;
using WorkoutApp.Application.DTOs.User.GetProfile;
using WorkoutApp.Application.Interfaces;
using WorkoutApp.Domain.Common;
using WorkoutApp.Domain.Repositories;

namespace WorkoutApp.Application.Features.Users.Queries.GetProfile;

public sealed class GetProfileQueryHandler(
    IUserRepository userRepository,
    ICurrentUserService currentUserService
) 
    : IRequestHandler<GetProfileQuery, Result<ProfileResponse>>
{
    public async Task<Result<ProfileResponse>> Handle(GetProfileQuery query, CancellationToken cancellationToken)
    {
        if (currentUserService.UserId is not { } userId)
            return Result.Failure<ProfileResponse>(
                Error.Unauthorized("User.Unauthorized", "You must be logged in to view your profile."));

        var userResult = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (userResult.IsFailure)
            return Result.Failure<ProfileResponse>(userResult.Error);

        var user = userResult.Value;
        return Result.Success(new ProfileResponse(user.Id, user.FirstName.Value, user.LastName.Value, user.Email.Value));
    }
}