using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Catalog.Core.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var commandName = request.GetGenericTypeName();

        _logger.LogInformation("----- Handling command '{CommandName}'", commandName);

        _logger.LogInformation("----- Request '{request}'", request);

        var timer = new Stopwatch();
        timer.Start();

        var response = await next();

        timer.Stop();

        _logger.LogInformation("----- Response", response.ToJson().ToString());

        var timeTaken = timer.Elapsed.TotalSeconds;
        _logger.LogInformation("----- Command '{CommandName}' handled ({TimeTaken} seconds)", commandName, timeTaken);

        return response;
    }
}