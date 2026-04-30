using aca_scaling_api.Contracts;
using aca_scaling_api.Interfaces;
using aca_scaling_api.Services.MessageGenerator;
using aca_scaling_api.Services.ServiceBus;
using aca_scaling_api.Validation;
using Azure;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace aca_scaling_api.Endpoints
{
    public static class SendMessagesEndpoint
    {
        private const string AzureEndpoint = "Replica Count";
        private const string TaskName = "GetReplicaCount";
        public static IEndpointRouteBuilder MapSendMessageEndpoint(this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/api/v1/send-message");

            group.MapGet("/{messageCount}",GenerateMessages);

            return endpoints;
        }

        private static async Task<IResult> GenerateMessages(
            int messageCount,
            [FromServices] IQueueService queueService,
            [FromServices] IMessageGenerator messageGenerator,
            [FromServices] IValidator<SendMessageRequest> validator,
            ILoggerFactory loggerFactory,
            HttpContext httpContext,
            CancellationToken cancellationToken)
        {
            var request = new SendMessageRequest(messageCount);
            var validationResult = await validator.ValidateAsync(request, cancellationToken).ConfigureAwait(false);

            if (!validationResult.IsValid)
            {
                var details = validationResult.Errors
                .GroupBy(error => string.IsNullOrWhiteSpace(error.PropertyName) ? "request" : error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.ErrorMessage).ToArray());

                return Results.Json(
                    new ApiError("BadRequest", "Invalid request parameters.", CorrelationId: httpContext.GetCorrelationId(), details),
                    statusCode: StatusCodes.Status400BadRequest
                );
            }
                        
            try
            {
                QueueContent queueLength = await queueService.GetQueueLength();
                
                _  = int.TryParse(queueLength.ActiveMessageCount, out int activeMessageCount);
                if (activeMessageCount != 0)
                {
                    return Results.Json(
                        new ApiError("Conflict", $"Queue is not empty. Current active message count: {queueLength.ActiveMessageCount}.", CorrelationId: httpContext.GetCorrelationId()),
                        statusCode: StatusCodes.Status429TooManyRequests
                    );
                }

                IEnumerable<MessageContent> generatedMessages = await messageGenerator.GenerateMessagesToQueueAsync(messageCount, httpContext.GetCorrelationId());

                await queueService.SendMessageAsync(generatedMessages);

                return Results.Accepted(generatedMessages.Count().ToString());
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
