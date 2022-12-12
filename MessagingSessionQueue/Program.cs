using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using StackExchange.Redis;

namespace MessagingSessionQueue
{
    public class Program
    {
        private static readonly string _connString = Environment.GetEnvironmentVariable("CUSTOMCONNSTR_ServiceBus");

        private const string _queueName = "session-queue";

        private const int _lineCount = 5, _lineSize = 5;

        private static  ServiceBusClient _serviceBusClient;

        private static readonly CountdownEvent _countdownEvent = new CountdownEvent(_lineCount * _lineSize);

        private static IDatabase _rediDB;

        public static async Task Main(string[] args)
        {
            try
            {
                ArgumentException.ThrowIfNullOrEmpty(_connString, nameof(_connString));

                await initQueueAsync();

                _serviceBusClient = new ServiceBusClient(_connString);

                await sendMessagesAsync();

                IEnumerable<ServiceBusSessionProcessor> listOfProcessors = await registerSessionHandlers(3);

                // Block the thread here. Waiting for the signal for all messages are processed.
                _countdownEvent.Wait();

                foreach (var processor in listOfProcessors)
                    await processor.StopProcessingAsync();

                await _serviceBusClient.DisposeAsync();

                Console.WriteLine("Done");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <param name="clientCount">The number of Processors works in parallel to simulate a distributed system.</param>
        private static async Task<IEnumerable<ServiceBusSessionProcessor>> registerSessionHandlers(int clientCount)
        {
            _rediDB = ConnectionMultiplexer.Connect("localhost:6379").GetDatabase();

            var listOfProcessors = new List<ServiceBusSessionProcessor>();

            var handlerOptions = new ServiceBusSessionProcessorOptions
            {
                MaxConcurrentSessions = 2,
                SessionIdleTimeout    = TimeSpan.FromSeconds(1),
                AutoCompleteMessages  = false,
                PrefetchCount         = 2
            };

            for (int i = 0; i < clientCount; i++)
            {
                ServiceBusSessionProcessor sessionProcessor = _serviceBusClient.CreateSessionProcessor(_queueName, handlerOptions);

                sessionProcessor.ProcessMessageAsync += processSessionMessagesAsync;
                sessionProcessor.ProcessErrorAsync   += processErrorHandlerAsync;

                await sessionProcessor.StartProcessingAsync();

                listOfProcessors.Add(sessionProcessor);
            }

            return listOfProcessors;
        }

        private static async Task processSessionMessagesAsync(ProcessSessionMessageEventArgs args)
        {
            // Amount of time until the Lock is expired. Renew the lock or cancel this operation. CancelToken can be created based on this time.
            TimeSpan timeToProcess = args.SessionLockedUntil - DateTime.UtcNow;

            ServiceBusReceivedMessage message = args.Message;

            string body = message.Body.ToString(); // Instead of Encoding.UTF8.GetString(message.Body), but doing it internally.

            string consoleMessage = $"Received Session: '{args.SessionId}' | TimeToProcess: {timeToProcess.Seconds} | SequenceNumber: {message.SequenceNumber} | Body: '{body}'.";

            // Simulate some work
            await Task.Delay(Random.Shared.Next(500, 1_000), args.CancellationToken);

            if (Random.Shared.NextDouble() <= 0.05)
            {
                await args.AbandonMessageAsync(message);

                Console.WriteLine("Abandon - " + consoleMessage);

                //throw new Exception("My Exception");
            }
            else
            {
                await args.CompleteMessageAsync(message);

                Console.WriteLine(consoleMessage);

                await _rediDB.ListRightPushAsync(message.SessionId, body); // Check values in Redis: LRANGE Line-1 0 -1

                _countdownEvent.Signal();
            }
        }

        private static async Task sendMessagesAsync()
        {
            ServiceBusSender messageSender = _serviceBusClient.CreateSender(_queueName);

            for (int i = 1; i <= _lineCount; i++)
            {
                string sessionId = $"Line-{i}";

                var messages = new List<ServiceBusMessage>(_lineSize);

                for (int j = 1; j <= _lineSize; j++)
                {
                    BinaryData body = BinaryData.FromString($"Pos-{j}"); // Instead of Encoding.UTF8.GetBytes, but doing it internally.

                    messages.Add(new ServiceBusMessage(body) { SessionId = sessionId });
                }

                await messageSender.SendMessagesAsync(messages);

                Console.WriteLine($"Sent: {sessionId}");
            }

            await messageSender.CloseAsync();
        }

        private static async Task initQueueAsync()
        {
            var adminClient = new ServiceBusAdministrationClient(_connString);

            if (await adminClient.QueueExistsAsync(_queueName))
                await adminClient.DeleteQueueAsync(_queueName);

            var createQueueOptions = new CreateQueueOptions(_queueName)
            {
                RequiresSession = true,
                LockDuration    = TimeSpan.FromSeconds(30)
            };

            await adminClient.CreateQueueAsync(createQueueOptions);
        }

        private static Task processErrorHandlerAsync(ProcessErrorEventArgs eventArgs)
        {
            if (eventArgs.Exception is OperationCanceledException)
                return Task.CompletedTask;

            Console.WriteLine($"Message handler encountered an exception {eventArgs.Exception}.");

            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- ErrorSource: {eventArgs.ErrorSource}");
            Console.WriteLine($"- EntityPath: {eventArgs.EntityPath}");
            Console.WriteLine($"- FullyQualifiedNamespace: {eventArgs.FullyQualifiedNamespace}");

            return Task.CompletedTask;
        }
    }
}