# Playing with Azure Service Bus
In this repository, I collected some useful links regarding the Azure Service Bus.

#### Projects: SessionQueue and MessagingSessionQueue

Imagine a situation, you are waiting in a line with others. Everyone has a position in the line when they arrived one after the other.

In this project, we are using the [Session feature](https://docs.microsoft.com/en-us/azure/service-bus-messaging/message-sessions). Push some messages in the queue with SessionId (line) and process them in the given order per session. 

#### Project: Playing with RabbitMQ

[In this separate repository](https://github.com/19balazs86/PlayingWithRabbitMQ#6-azure-service-bus) you can find a basic Producer / Consumer model for messaging.

### Service Bus - Resources

[Azure Service Bus Messaging documentation](https://learn.microsoft.com/en-us/azure/service-bus-messaging/) *(Microsoft)*

[Azure.Messaging.ServiceBus](https://github.com/Azure/azure-sdk-for-net/tree/Azure.Messaging.ServiceBus_7.11.1/sdk/servicebus/Azure.Messaging.ServiceBus) *(GitHub)* | [Examples](https://github.com/Azure/azure-sdk-for-net/tree/Azure.Messaging.ServiceBus_7.11.1/sdk/servicebus/Azure.Messaging.ServiceBus#examples) | [Troubleshooting](https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/servicebus/Azure.Messaging.ServiceBus/TROUBLESHOOTING.md)

[Azure.Messaging.ServiceBus Namespace](https://learn.microsoft.com/en-us/dotnet/api/azure.messaging.servicebus) *(Microsoft API reference)*

##### Guide for migrating to Azure.Messaging.ServiceBus from Microsoft.Azure.ServiceBus

[Migrating guide](https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/servicebus/Azure.Messaging.ServiceBus/MigrationGuide.md) *(GitHub)*

[Migrating to the New Azure Service Bus SDK](https://markheath.net/post/migrating-to-new-servicebus-sdk) *(Mark Heath)*

##### Deprecated - Service Bus - Resources

- [Microsoft.Azure.ServiceBus Namespace](https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.servicebus?view=azure-dotnet)
- GitHub: [Samples](https://github.com/Azure/azure-service-bus/tree/master/samples/DotNet) | [Microsoft.Azure.ServiceBus source code](https://github.com/Azure/azure-sdk-for-net/tree/master/sdk/servicebus/Microsoft.Azure.ServiceBus/src)
- [Using Azure Service Bus Queues and Topics series](https://damienbod.com/2019/04/23/using-azure-service-bus-queues-with-asp-net-core-services/) *(Damien Bod)*

##### Processing messages/events in the given order with Azure Functions *(Jeff Hollan)*

- [Choosing between queues and event hubs](https://hackernoon.com/azure-functions-choosing-between-queues-and-event-hubs-dac4157eee1c)
- [Event processing with Event Hubs](https://medium.com/@jeffhollan/in-order-event-processing-with-azure-functions-bb661eb55428)
- [Ordered queue processing with Sessions](https://dev.to/azure/ordered-queue-processing-in-azure-functions-4h6c)
- [Reliable Event Processing](https://hackernoon.com/reliable-event-processing-in-azure-functions-37054dc2d0fc)

