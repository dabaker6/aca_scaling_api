# ACA Scaling API

## Overview

ACA Scaling API is a .NET 10 Worker Service designed to interact with Azure Container Apps and Azure Service Bus. It provides APIs and background services for monitoring, scaling, and message queue operations in cloud-native environments.

## Features

- Query and manage Azure Container Apps, including scaling operations.
- Integrate with Azure Service Bus for queue-based messaging.
- Expose endpoints for sending messages and monitoring queue length.
- Built with extensibility and cloud deployment in mind.

## Technologies

- .NET 10 Worker Service
- Azure Container Apps SDK
- Azure Service Bus SDK
- Serilog for logging
- Docker support for containerized deployment

## Getting Started

1. Clone the repository.
2. Configure your Azure credentials and settings in `appsettings.json`.
3. Build and run the service:
4. (Optional) Build and run with Docker:
5. Ensure correct permissions are assigned in Azure. Azure Service Bus Data Owner is required to monitor queue length
## Configuration

- `ServiceBusSettings`: Configure your Azure ServiceBus connection and queue name.
- `ContainerAppsSettings`: Set your Azure subscription, resource group, and container app details.

## Endpoints

- Message sending and queue monitoring endpoints are available (see`SendMessagesEndpoint.cs`).

## License

This projectis licensed under the MIT License.