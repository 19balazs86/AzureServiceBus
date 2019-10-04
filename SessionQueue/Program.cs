using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.Management;

namespace SessionQueue
{
  public class Program
  {
    private static readonly string _connString = Environment.GetEnvironmentVariable("CUSTOMCONNSTR_ServiceBus");

    private const string _queueName = "session-queue";

    private const int _lineCount = 10, _lineSize = 20;

    public static async Task Main(string[] args)
    {
      try
      {
        await initQueueAsync();

        await sendMessagesAsync();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
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
  }
}
