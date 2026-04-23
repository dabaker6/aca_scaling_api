using aca_scaling_api.Contracts;
using aca_scaling_api.Services.ContainerApps;
using aca_scaling_api.Validation;
using Azure;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace aca_scaling_api.Endpoints
{
    public static class GetReplicaCountEndpoint
    {
        private const string AzureEndpoint = "Replica Count";
        private const string TaskName = "GetReplicaCount";
        public static IEndpointRouteBuilder MapGetReplicaCountEndpoint(this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/api/v1/replicas");

            group.MapGet("/{revisionName}", GetReplicaCount);

            return endpoints;
        }

        private static async Task<IResult> GetReplicaCount(
            string revisionName,
            [FromServices] IContainerAppsService containerAppsService,
            [FromServices] ILoggerFactory loggerFactory,
            [FromServices] IValidator<ReplicaCountRequest> validator,
            HttpContext httpContext,
            CancellationToken cancellationToken)
        {
            var request = new ReplicaCountRequest(revisionName);
            var validationResult = await validator.ValidateAsync(request, cancellationToken).ConfigureAwait(false);

            if (!validationResult.IsValid)
            {
                var details = validationResult.Errors
                .GroupBy(error => string.IsNullOrWhiteSpace(error.PropertyName) ? "request" : error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.ErrorMessage).ToArray());
                
                return Results.Json(
                    new ApiError("BadRequest", "Invalid request parameters.", CorrelationId: httpContext.GetCorrelationId(),details),
                    statusCode: StatusCodes.Status400BadRequest
                );
            }

            try 
            {
                var replicaCount = await containerAppsService.GetReplicaCount(revisionName, cancellationToken);

                return Results.Ok(replicaCount);
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
