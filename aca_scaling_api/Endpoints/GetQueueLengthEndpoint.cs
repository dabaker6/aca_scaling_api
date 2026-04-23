using aca_scaling_api.Contracts;
using aca_scaling_api.Services.ServiceBus;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace aca_scaling_api.Endpoints
{
    public static class GetQueueLengthEndpoint
    {
        private const string AzureEndpoint = "Queue Length";
        private const string TaskName = "GetQueueLength";

        public static IEndpointRouteBuilder MapGetQueueLengthEndpoint(this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/api/v1/queue-length");

            group.MapGet("/", GetQueueLength);

            return endpoints;
        }

        private static async Task<IResult> GetQueueLength(
            [FromServices] IQueueService queueService,
            [FromServices] ILoggerFactory loggerFactory,
            HttpContext httpContext,
            CancellationToken cancellationToken)
        {
            try
            {
                var queueContent = await queueService.GetQueueLength();
                return Results.Ok(queueContent);
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

