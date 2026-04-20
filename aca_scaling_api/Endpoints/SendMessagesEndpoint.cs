using aca_scaling_api.Infrastructure.ServiceBus;
using aca_scaling_api.Utils;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;

namespace aca_scaling_api.Endpoints
{
    public static class SendMessagesEndpoint
    {
        public static IEndpointRouteBuilder MapSendMessageEndpoint(this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/api/v1/send-message");

            group.MapGet("/",GenerateMessages);

            return endpoints;
        }

        private static async Task<IResult> GenerateMessages(
            [FromServices] IQueueService queueService,
            HttpContext httpContext,
            CancellationToken cancellationToken)
        {
            var generatedMessages = await MessageGenerator.GenerateMessagesToQueue();


            foreach (var generatedMessage in generatedMessages)
            {
                await queueService.SendMessageAsync(System.Text.Json.JsonSerializer.Serialize(generatedMessage));
            }

            return Results.Accepted(generatedMessages.Count().ToString());
        }
    }
}
