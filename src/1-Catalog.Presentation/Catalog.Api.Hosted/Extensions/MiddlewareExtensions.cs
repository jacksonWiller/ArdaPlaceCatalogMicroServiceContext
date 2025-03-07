using Catalog.Api.Hosted.Middlewares;

namespace Catalog.Api.Hosted.Extensions;

internal static class MiddlewareExtensions
{
    public static void UseCorrelationId(this IApplicationBuilder builder) =>
        builder.UseMiddleware<CorrelationIdMiddleware>();

    public static void UseErrorHandling(this IApplicationBuilder builder) =>
        builder.UseMiddleware<ErrorHandlingMiddleware>();
}