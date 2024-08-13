using FluentValidation;
using Handler.Api;
using Handler.Api.Data;
using Handler.Api.Decorators;
using Handler.Api.Features.Users.CreateUser;
using Handler.Api.Features.Users.GetUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddValidatorsFromAssembly(typeof(IApplication).Assembly);


builder.Services.AddMediatR(cfg =>
{
    // register all request handlers in this assembly
    cfg.RegisterServicesFromAssembly(typeof(IApplication).Assembly);
    
    // decorate all handlers with a logging decorator
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(RequestLoggingDecorator<,>));
    
    // decorate all handlers with a validation decorator
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(RequestValidationDecorator<,>));
});

// register a custom exception handler to handle validation and notfound exception
builder.Services.AddExceptionHandler<ExceptionHandler>();

// register a in memory database for test purposes
builder.Services.AddDbContext<ApplicationDbContext>(cfg => { cfg.UseInMemoryDatabase("users"); });
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseExceptionHandler(_ => { });


app.MapGet("/users/{id:guid}", async ([FromRoute] Guid id, [FromServices] ISender sender) =>
    {
        var user = await sender.Send(new GetUserRequest()
        {
            Id = id
        });
        return Results.Ok(user);
    }).WithName("GetUser")
    .WithOpenApi();

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

app.Run();