using Ardalis.GuardClauses;
using Handler.Api.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Handler.Api.Features.Users.GetUser;

public class GetUserRequestHandler(ApplicationDbContext dbContext) : IRequestHandler<GetUserRequest, UserDto>
{
    public async Task<UserDto> Handle(GetUserRequest request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id ==request.Id, cancellationToken: cancellationToken);

        Guard.Against.NotFound(request.Id, user);

        return new UserDto()
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.UserName
        };
    }
}