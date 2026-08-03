using MediatR;
using WorkoutApp.Application.Interfaces;
using WorkoutApp.Domain.Common;
using WorkoutApp.Domain.Repositories;

namespace WorkoutApp.Application.Features.Users.Commands.DeleteAccount;

public sealed class DeleteAccountCommandHandler(
    IUserRepository userRepository,
    IWorkoutRepository workoutRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ICurrentUserService currentUserService
) 
    : IRequestHandler<DeleteAccountCommand, Result>
{
    public async Task<Result> Handle(DeleteAccountCommand command, CancellationToken cancellationToken)
    {
        if (currentUserService.UserId is not { } userId)
            return Result.Failure(Error.Unauthorized("User.Unauthorized", "You must be logged in to delete your account."));

        var userResult = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (userResult.IsFailure)
            return Result.Failure(userResult.Error);

        var user = userResult.Value;

        if (!passwordHasher.Verify(command.Request.Password, user.PasswordHash))
            return Result.Failure(Error.Unauthorized("User.InvalidPassword", "The password you entered is incorrect."));

        var workouts = await workoutRepository.GetByUserAsync(userId, cancellationToken);
        foreach (var workout in workouts)
            workout.Delete();

        user.Delete();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}