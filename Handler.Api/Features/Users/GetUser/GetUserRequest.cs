using MediatR;

namespace Handler.Api.Features.Users.GetUser;

public class GetUserRequest : IRequest<UserDto>
{
    public Guid Id { get; set; }
}