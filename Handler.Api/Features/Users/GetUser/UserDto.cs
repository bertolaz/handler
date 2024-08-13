namespace Handler.Api.Features.Users.GetUser;

public record UserDto
{
    public required Guid Id  { get; init; }
    public required string Email { get; init; }
    public required string Username { get; init; }
}