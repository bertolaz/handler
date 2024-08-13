using MediatR;

namespace Handler.Api.Decorators;

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
        catch(Exception e)
        {
            logger.LogError("Request {Request} failed: {Message}", request.GetType().Name, e.Message);
            throw;
        }
    }
}