using MediatR;
using WorkoutApp.Application.DTOs.User.DeleteAccount;
using WorkoutApp.Domain.Common;

namespace WorkoutApp.Application.Features.Users.Commands.DeleteAccount;

public sealed record DeleteAccountCommand(DeleteAccountRequest Request) 
    : IRequest<Result>;