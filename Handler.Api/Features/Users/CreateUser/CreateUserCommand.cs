using MediatR;

namespace Handler.Api.Features.Users.CreateUser;

public class CreateUserCommand : IRequest<Guid>
{
    public required string Email { get; set; }
    public required string Username { get; set; }
}