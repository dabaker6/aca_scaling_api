using aca_scaling_api.Contracts;
using aca_scaling_api.Services.ContainerApps;
using Azure;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace aca_scaling_api.Endpoints
{
    public static class GetRevisionNameEndpoint
    {
        private const string AzureEndpoint = "Revision Name";
        private const string TaskName = "GetRevisionName";

        public static IEndpointRouteBuilder MapGetRevisionNameEndpoint(this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/api/v1/revisionName");

            group.MapGet("/", GetRevisionName);

            return endpoints;
        }

        private static async Task<IResult> GetRevisionName(
            [FromServices] IContainerAppsService containerAppsService,
            [FromServices] ILoggerFactory loggerFactory,
            HttpContext httpContext,
            CancellationToken cancellationToken)
        {
            try
            {
                var revisionName = await containerAppsService.GetRevisionName(cancellationToken);

                if (revisionName is null)
                {
                   var notFoundError = new ApiError(
                   HttpStatusCode.NotFound.ToString(),
                   $"Revision not found: {revisionName}",
                   httpContext.GetCorrelationId());

                   return Results.NotFound(notFoundError);
                }           

                return Results.Ok(revisionName);
            }
            catch (RequestFailedException ex)
            {
                var logger = loggerFactory.CreateLogger(TaskName);
                logger.LogError(ex, "Request Failed");

                return Results.Json(
                    new ApiError(ex.Status.ToString(), $"Failed to get {AzureEndpoint}", CorrelationId: httpContext.GetCorrelationId()),
                    statusCode: ex.Status
                    );
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger(TaskName);
                logger.LogError(ex, $"An unexpected error occurred while getting the {AzureEndpoint}.");
                return Results.Json(
                    new ApiError("InternalServerError", "An unexpected error occurred.", CorrelationId: httpContext.GetCorrelationId()),
                    statusCode: StatusCodes.Status500InternalServerError
                    );
            }
        }
    }
}
