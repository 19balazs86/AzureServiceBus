using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.Management;
using StackExchange.Redis;

namespace SessionQueue
{
  public class Program
  {
    private static readonly string _connString = Environment.GetEnvironmentVariable("CUSTOMCONNSTR_ServiceBus");

    private const string _queueName = "session-queue";

    private const int _lineCount = 5, _lineSize = 5;

    private static readonly CountdownEvent _countdownEvent = new CountdownEvent(_lineCount * _lineSize);

    private static readonly Random _random = new Random();

    private static IDatabase _rediDB;

    public static async Task Main(string[] args)
    {
      try
      {
        await initQueueAsync();

        await sendMessagesAsync();

        registerSessionHandlers();

        // Block the thread here. Waiting for the signal for all messages are processed.
        _countdownEvent.Wait();

        Console.WriteLine("Done");
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }

    private static void registerSessionHandlers()
    {
      _rediDB = ConnectionMultiplexer.Connect("localhost:6379").GetDatabase();

      int clientCount = 3; // The number of QueueClient works in parallel to simulate a distributed system.

      for (int i = 0; i < clientCount; i++)
      {
        var queueClient = new QueueClient(_connString, _queueName);

        var handlerOptions = new SessionHandlerOptions(exceptionHandlerAsync)
        {
          // Maximum number of concurrent calls to the callback "processSessionMessagesAsync".
          // The method can be called with a message for 2 unique session Id's in parallel.
          MaxConcurrentSessions = 2,
          MessageWaitTimeout    = TimeSpan.FromSeconds(1),
          AutoComplete          = false
        };

        queueClient.RegisterSessionHandler(processSessionMessagesAsync, handlerOptions);
      }
    }

    private static async Task processSessionMessagesAsync(
      IMessageSession session,
      Message message,
      CancellationToken cancelToken)
    {
      string body = Encoding.UTF8.GetString(message.Body);

      //string consoleMessage = $"Received Session: '{session.SessionId}' | SequenceNumber: {message.SystemProperties.SequenceNumber} | Body: '{body}'.";

      if (_random.NextDouble() <= 0.05)
      {
        await session.AbandonAsync(message.SystemProperties.LockToken);

        //Console.WriteLine("Abandon - " + consoleMessage);
      }
      else
      {
        await session.CompleteAsync(message.SystemProperties.LockToken);

        //Console.WriteLine(consoleMessage);

        await _rediDB.ListRightPushAsync(message.SessionId, body); // Check values: LRANGE Line-1 0 -1

        _countdownEvent.Signal();
      }
    }

    private static async Task sendMessagesAsync()
    {
      var messageSender = new MessageSender(_connString, _queueName);

      for (int i = 0; i < _lineCount; i++)
      {
        string sessionId = $"Line-{i}";

        for (int j = 0; j < _lineSize; j++)
        {
          byte[] body = Encoding.UTF8.GetBytes($"Pos-{j}");

          Message message = new Message(body) { SessionId = sessionId };

          await messageSender.SendAsync(message);
        }

        Console.WriteLine($"Sent: {sessionId}.");
      }

      await messageSender.CloseAsync();
    }

    private static async Task initQueueAsync()
    {
      var managementClient = new ManagementClient(_connString);

      if (await managementClient.QueueExistsAsync(_queueName))
        await managementClient.DeleteQueueAsync(_queueName);

      var queueDescription = new QueueDescription(_queueName) { RequiresSession = true };

      await managementClient.CreateQueueAsync(queueDescription);

      await managementClient.CloseAsync();
    }

    private static Task exceptionHandlerAsync(ExceptionReceivedEventArgs eventArgs)
    {
      if (eventArgs.Exception is OperationCanceledException)
        return Task.CompletedTask;

      Console.WriteLine($"Message handler encountered an exception {eventArgs.Exception}.");

      var context = eventArgs.ExceptionReceivedContext;

      Console.WriteLine("Exception context for troubleshooting:");
      Console.WriteLine($"- Endpoint: {context.Endpoint}");
      Console.WriteLine($"- Entity Path: {context.EntityPath}");
      Console.WriteLine($"- Executing Action: {context.Action}");

      return Task.CompletedTask;
    }
  }
}
