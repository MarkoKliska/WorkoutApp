using MediatR;
using WorkoutApp.Application.DTOs.User.GetProfile;
using WorkoutApp.Application.Interfaces;
using WorkoutApp.Domain.Common;
using WorkoutApp.Domain.Enums;
using WorkoutApp.Domain.Repositories;

namespace WorkoutApp.Application.Features.Users.Commands.UpdateProfile;

public sealed class UpdateProfileCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService
) 
    : IRequestHandler<UpdateProfileCommand, Result<ProfileResponse>>
{
    public async Task<Result<ProfileResponse>> Handle(UpdateProfileCommand command, CancellationToken cancellationToken)
    {
        if (currentUserService.UserId is not { } userId)
            return Result.Failure<ProfileResponse>(
                Error.Unauthorized("User.Unauthorized", "You must be logged in to update your profile."));

        var userResult = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (userResult.IsFailure)
            return Result.Failure<ProfileResponse>(userResult.Error);

        var user = userResult.Value;
        var request = command.Request;

        if (!string.Equals(user.Email.Value, request.Email, StringComparison.OrdinalIgnoreCase))
        {
            var existingUser = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingUser.IsSuccess)
                return Result.Failure<ProfileResponse>(
                    Error.Conflict("User.EmailAlreadyExists", "An account with this email already exists."));

            if (existingUser.Error.Type != ErrorType.NotFound)
                return Result.Failure<ProfileResponse>(existingUser.Error);
        }

        var updateResult = user.UpdateProfile(request.FirstName, request.LastName, request.Email);
        if (updateResult.IsFailure)
            return Result.Failure<ProfileResponse>(updateResult.Error);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new ProfileResponse(user.Id, user.FirstName.Value, user.LastName.Value, user.Email.Value));
    }
}