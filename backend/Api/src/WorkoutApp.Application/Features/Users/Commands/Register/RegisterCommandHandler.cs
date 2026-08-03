using MediatR;
using Microsoft.Extensions.Logging;
using WorkoutApp.Application.DTOs.User.RegisterUser;
using WorkoutApp.Application.Features.Users.Events;
using WorkoutApp.Application.Interfaces;
using WorkoutApp.Domain.Common;
using WorkoutApp.Domain.Entities;
using WorkoutApp.Domain.Enums;
using WorkoutApp.Domain.Repositories;

namespace WorkoutApp.Application.Features.Users.Commands.Register;

public sealed class RegisterCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IPublisher publisher,
    ILogger<RegisterCommandHandler> logger
)
    : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    public async Task<Result<RegisterResponse>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var existingUser = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser.IsSuccess)
            return Result.Failure<RegisterResponse>(
                Error.Conflict("User.EmailAlreadyExists", "An account with this email already exists."));

        if (existingUser.Error.Type != ErrorType.NotFound)
            return Result.Failure<RegisterResponse>(existingUser.Error);

        var passwordHash = passwordHasher.Hash(request.Password);

        var userResult = User.Register(request.FirstName, request.LastName, request.Email, passwordHash);
        if (userResult.IsFailure)
            return Result.Failure<RegisterResponse>(userResult.Error);

        var user = userResult.Value;
        userRepository.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            await publisher.Publish(
                new UserRegisteredEvent(user.Id, user.Email.Value, user.FirstName.Value, user.LastName.Value),
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Failed to publish UserRegisteredEvent for user {UserId}. The account was created but the welcome email will not be sent.",
                user.Id);
        }

        var token = tokenService.GenerateToken(user);

        return Result.Success(new RegisterResponse(user.Id, token));
    }
}