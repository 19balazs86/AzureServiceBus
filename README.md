# Playing with Azure Service Bus
In this repository, I collected some useful links regarding the Azure Service Bus.

#### Project: SessionQueue

Imagine a situation, you are waiting in a line with others. Everyone has a position in the line when they arrived one after the other.

In this project, I am using the [Session feature](https://docs.microsoft.com/en-us/azure/service-bus-messaging/message-sessions), which you can enable for only queue. The application is very simple. Push some messages (position) in the queue with SessionId (line) and process them in the given order for each session (line). 

#### Project: Playing with RabbitMQ

[In this separate repository](https://github.com/19balazs86/PlayingWithRabbitMQ#6-azure-service-bus) you can find a basic Producer / Consumer model for messaging.

#### Service Bus - Resources

- Microsoft: [Documentation](https://docs.microsoft.com/en-us/azure/service-bus-messaging/) | [Microsoft.Azure.ServiceBus Namespace](https://docs.microsoft.com/en-us/dotnet/api/microsoft.azure.servicebus?view=azure-dotnet)
- GitHub: [Samples](https://github.com/Azure/azure-service-bus/tree/master/samples/DotNet) | [Microsoft.Azure.ServiceBus source code](https://github.com/Azure/azure-sdk-for-net/tree/master/sdk/servicebus/Microsoft.Azure.ServiceBus/src)
- [Using Azure Service Bus Queues and Topics series](https://damienbod.com/2019/04/23/using-azure-service-bus-queues-with-asp-net-core-services/) *(Damien Bod)*
- [Migrating to the New Azure Service Bus SDK](https://markheath.net/post/migrating-to-new-servicebus-sdk) *(Mark Heath)*

##### Processing messages/events in the given order with Azure Functions *(Jeff Hollan)*

- [Choosing between queues and event hubs](https://hackernoon.com/azure-functions-choosing-between-queues-and-event-hubs-dac4157eee1c)
- [Event processing with Event Hubs](https://medium.com/@jeffhollan/in-order-event-processing-with-azure-functions-bb661eb55428)
- [Ordered queue processing with Sessions](https://dev.to/azure/ordered-queue-processing-in-azure-functions-4h6c)
- [Reliable Event Processing](https://hackernoon.com/reliable-event-processing-in-azure-functions-37054dc2d0fc)