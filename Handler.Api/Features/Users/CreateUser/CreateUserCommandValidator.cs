using FluentValidation;

namespace Handler.Api.Features.Users.CreateUser;

public class CreateUserCommandValidator: AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Username).NotEmpty();
    }
}