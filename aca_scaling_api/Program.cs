using aca_scaling_api.Configuration;
using aca_scaling_api.Contracts;
using aca_scaling_api.Endpoints;
using aca_scaling_api.Services;
using aca_scaling_api.Services.ContainerApps;
using aca_scaling_api.Services.ServiceBus;
using aca_scaling_api.Validation;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Azure.ResourceManager;
using FluentValidation;
using Serilog;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture));

builder.Services
    .AddOptions<ServiceBusSettings>()
    .Bind(builder.Configuration.GetSection(ServiceBusSettings.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddOptions<ContainerAppsSettings>()
    .Bind(builder.Configuration.GetSection(ContainerAppsSettings.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton(sp =>
{
    var fullyQualifiedNamespace = builder.Configuration["ServiceBus:FullyQualifiedNamespace"];
    return new ServiceBusClient(
    fullyQualifiedNamespace,
    new DefaultAzureCredential());
});

builder.Services.AddSingleton(sp =>
{
    var fullyQualifiedNamespace = builder.Configuration["ServiceBus:FullyQualifiedNamespace"];
    return new ServiceBusAdministrationClient(
    fullyQualifiedNamespace,
    new DefaultAzureCredential());
});

builder.Services.AddSingleton(sp =>
{
    return new ArmClient(new DefaultAzureCredential());
});

builder.Services.AddValidatorsFromAssemblyContaining<ReplicaCountRequestValidator>(includeInternalTypes: true);
builder.Services.AddSingleton<IQueueService, QueueService>();
builder.Services.AddSingleton<IContainerAppsService, ContainerAppsService>();
builder.Services.AddSingleton<IMessageGenerator, MessageGeneratorService>();

var app = builder.Build();

app.Use(async (context, next) =>
{
    var incomingCorrelationId = context.Request.Headers[CorrelationConstants.HeaderName].FirstOrDefault();
    var correlationId = string.IsNullOrWhiteSpace(incomingCorrelationId)
        ? context.TraceIdentifier
        : incomingCorrelationId.Trim();

    context.TraceIdentifier = correlationId;
    context.Items[CorrelationConstants.ItemKey] = correlationId;

    context.Response.OnStarting(() =>
    {
        context.Response.Headers[CorrelationConstants.HeaderName] = correlationId;
        return Task.CompletedTask;
    });

    await next().ConfigureAwait(false);
});

app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("CorrelationId", httpContext.GetCorrelationId());
        diagnosticContext.Set("RequestMethod", httpContext.Request.Method);
        diagnosticContext.Set("RequestPath", httpContext.Request.Path.Value ?? "/");
        diagnosticContext.Set("EndpointName", httpContext.GetEndpoint()?.DisplayName ?? "unknown");
    };
});

app.MapGet("/", () => "Hello World!");

app.MapSendMessageEndpoint();
app.MapGetReplicaCountEndpoint();
app.MapGetRevisionNameEndpoint();
app.MapGetQueueLengthEndpoint();

app.Run();
