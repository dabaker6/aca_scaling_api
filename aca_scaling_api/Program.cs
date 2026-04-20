using aca_scaling_api.Configuration;
using aca_scaling_api.Endpoints;
using aca_scaling_api.Infrastructure.ServiceBus;
using Azure.Identity;
using Azure.Messaging.ServiceBus;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOptions<ServiceBusSettings>()
    .Bind(builder.Configuration.GetSection(ServiceBusSettings.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton(sp =>
{
    var fullyQualifiedNamespace = builder.Configuration["ServiceBus:FullyQualifiedNamespace"];
    return new ServiceBusClient(
    fullyQualifiedNamespace,
    new DefaultAzureCredential());
});

builder.Services.AddSingleton<IQueueService, QueueService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapSendMessageEndpoint();

app.Run();
