using MediatR;
using WorkoutApp.Application.DTOs.User.RegisterUser;
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
    ITokenService tokenService
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

        var token = tokenService.GenerateToken(user);

        return Result.Success(new RegisterResponse(user.Id, token));
    }
}