# Request handler example
Spooners go the Code section below :)
## Description
In this example you can find a web api application that handles the following requests:
- Create a user Hanlder.Api/Features/Users/CreateUser
- Get a user by id Hanlder.Api/Features/Users/GetUser

The application uses an in-memory database to store the users. The database is created when the application starts and is destroyed when the application stops.

## How to run
To run the application run the following docker commands
```
bash docker build . -t <name of the tag>
```
```
docker run -p 8080:8080 <name of the tag>
```

To create a user send a POST request to http://localhost:8080/users with the following body:
```
{
    "name": "John",
    "email": "john@example.com"
}
```

To get a user by id send a GET request to http://localhost:8080/users/{id} where {id} is the id of the user you want to get.

## Cool things to note

### Dependency injection (Inversion of control)
If you look Program.cs you will see that the application registers all the dependencies in the DI container. 
This makes the code more testable and easier to maintain.

### Mediator pattern using MediatR Library
The application uses the MediatR library to handle the requests. 
This library makes it easy to add new requests and to test the existing ones.
The features of the application are contained in the Features folder. Each feature has a request, a handler and a response. This makes it easy to add new features to the application.

### FluentValidation
The application uses the FluentValidation library to validate the requests.
This makes the code cleaner and easier to read.

### Decorators
Every request is wrapped with a loggin decorator that logs the request
Every request is wrapped with a validation decorator that validates the request


## Code
Some code to share with you

### Api definition
```csharp
app.MapPost("/users", async ([FromBody] CreateUserCommand command, [FromServices] ISender sender) =>
    {
        var res = await sender.Send(new CreateUserCommand
        {
            Email = command.Email,
            Username = command.Username
        });
        return Results.Created("/users", res);
    })
    .WithName("CreateUser")
    .WithOpenApi();

```

### Request definition
```csharp
// request definition
public class CreateUserCommand : IRequest<Guid>
{
    public required string Email { get; set; }
    public required string Username { get; set; }
}

// Validate the request
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
        
        // commit changes to the database
        await dbContext.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
```

### Request wrappers (decorators)
```csharp

public class RequestLoggingDecorator<TRequest, TResult>(ILogger<RequestLoggingDecorator<TRequest, TResult>> logger)
    : IPipelineBehavior<TRequest, TResult> where TRequest : notnull
{
    public async Task<TResult> Handle(TRequest request, RequestHandlerDelegate<TResult> next,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Processing Request {Request}", request.GetType().Name);
            var response = await next();
            logger.LogInformation("Request {Request} processed successfully", request.GetType().Name);
            return response;
        }
        catch
        {
            logger.LogError("Request {Request} failed", request.GetType().Name);
            throw;
        }
    }
}

public class RequestValidationDecorator<TRequest, TResult>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResult> where TRequest : notnull
{
    public async Task<TResult> Handle(TRequest request, RequestHandlerDelegate<TResult> next, CancellationToken cancellationToken)
    {
        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                validators.Select(v =>
                    v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Any())
                throw new ValidationException(failures);
        }
        return await next();
    }
}
```







