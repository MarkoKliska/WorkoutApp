using MediatR;
using WorkoutApp.Application.DTOs.User.LoginUser;
using WorkoutApp.Application.Interfaces;
using WorkoutApp.Domain.Common;
using WorkoutApp.Domain.Repositories;

namespace WorkoutApp.Application.Features.Users.Queries.Login;

public sealed class LoginQueryHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService) : IRequestHandler<LoginQuery, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginQuery query, CancellationToken cancellationToken)
    {
        var request = query.Request;

        var userResult = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (userResult.IsFailure)
            return Result.Failure<LoginResponse>(Error.Unauthorized("User.InvalidCredentials", "Invalid email or password."));

        var user = userResult.Value;

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result.Failure<LoginResponse>(Error.Unauthorized("User.InvalidCredentials", "Invalid email or password."));

        var token = tokenService.GenerateToken(user);

        return Result.Success(new LoginResponse(user.Id, token));
    }
}