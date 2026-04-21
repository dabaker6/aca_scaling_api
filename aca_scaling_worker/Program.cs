using aca_scaling_api.Configuration;
using aca_scaling_worker;
using Azure.Identity;
using Azure.Messaging.ServiceBus;

var builder = Host.CreateApplicationBuilder(args);


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

builder.Services.AddHostedService<ServiceBusWorker>();

var host = builder.Build();
host.Run();
