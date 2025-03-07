using Catalog.Api.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Catalog.Api.Hosted.Extensions;

internal static class MiddlewareExtensions
{
    public static void UseCorrelationId(this IApplicationBuilder builder) =>
        builder.UseMiddleware<CorrelationIdMiddleware>();

    public static void UseErrorHandling(this IApplicationBuilder builder) =>
        builder.UseMiddleware<ErrorHandlingMiddleware>();
}