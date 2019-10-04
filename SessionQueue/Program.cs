using System;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus.Management;

namespace SessionQueue
{
  public class Program
  {
    private static readonly string _connString = Environment.GetEnvironmentVariable("CUSTOMCONNSTR_ServiceBus");

    private const string _queueName = "session-queue";

    public static async Task Main(string[] args)
    {
      try
      {
        await initQueueAsync();
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
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
