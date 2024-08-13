using FluentValidation;
using MediatR;

namespace Handler.Api.Decorators;

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