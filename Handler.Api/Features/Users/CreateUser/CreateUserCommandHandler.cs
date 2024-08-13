using Handler.Api.Data;
using MediatR;

namespace Handler.Api.Features.Users.CreateUser;

public class CreateUserCommandHandler(ApplicationDbContext dbContext) : IRequestHandler<CreateUserCommand, Guid>
{
    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            UserName = request.Username
        };

        await dbContext.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}