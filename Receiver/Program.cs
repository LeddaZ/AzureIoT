using System.Text;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace Receiver
{
    class Program
    {
        private static readonly string cosmosEndpointUri =
            Environment.GetEnvironmentVariable("COSMOS_ENDPOINT") ?? "";
        private static readonly string cosmosPrimaryKey =
            Environment.GetEnvironmentVariable("COSMOS_KEY") ?? "";
        private static readonly string cosmosDatabase =
            Environment.GetEnvironmentVariable("DB_NAME") ?? "";
        private static readonly string cosmosContainerId =
            Environment.GetEnvironmentVariable("CONTAINER_NAME") ?? "";
        private static readonly string HostName =
            Environment.GetEnvironmentVariable("HOST_NAME") ?? "";
        private static readonly string DeviceId =
            Environment.GetEnvironmentVariable("DEVICE_ID") ?? "";
        private static readonly string AccessKey =
            Environment.GetEnvironmentVariable("ACCESS_KEY") ?? "";
        private static readonly string connectionString =
            "HostName="
            + HostName
            + ";SharedAccessKeyName=iothubowner;SharedAccessKey="
            + AccessKey
            + ";DeviceId="
            + DeviceId;

        private static DeviceClient deviceClient;

        static async Task Main(string[] args)
        {
            deviceClient = DeviceClient.CreateFromConnectionString(
                connectionString,
                TransportType.Mqtt
            );

            Console.WriteLine("Device is listening for messages...");
            await ReceiveMessageAsync();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static async Task ReceiveMessageAsync()
        {
            while (true)
            {
                var receivedMessage = await deviceClient.ReceiveAsync(TimeSpan.FromSeconds(10));

                if (receivedMessage != null)
                {
                    string messageBody = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                    Console.WriteLine($"Received message: {messageBody}");

                    try
                    {
                        Message message = JsonConvert.DeserializeObject<Message>(messageBody);
                        await UpdateReceivedAsync(message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                    await deviceClient.CompleteAsync(receivedMessage);
                }

                await Task.Delay(1000);
            }
        }

        private static async Task UpdateReceivedAsync(Message message)
        {
            try
            {
                var queryDefinition = new QueryDefinition(
                    "SELECT * FROM c WHERE c.MessaggioID = @messaggioId"
                ).WithParameter("@messaggioId", message.MessageID);
                Console.WriteLine(message.MessageID);

                var cosmosClient = new CosmosClient(cosmosEndpointUri, cosmosPrimaryKey);
                var database = cosmosClient.GetDatabase(cosmosDatabase);
                var container = database.GetContainer(cosmosContainerId);
                var iterator = container.GetItemQueryIterator<Message>(queryDefinition);
                var response = await iterator.ReadNextAsync();

                if (response.Count != 0)
                {
                    var existingMessage = response.First();
                    existingMessage.Received = true;

                    await container.ReplaceItemAsync(existingMessage, existingMessage.id);
                    Console.WriteLine("Message updated successfully.");
                }
                else
                {
                    Console.WriteLine("Message not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public class Message
        {
            public required string id { get; set; }
            public required string MessageID { get; set; }
            public required string DeviceId { get; set; }
            public double Value { get; set; }
            public DateTime DataRicezione { get; set; }
            public bool Received { get; set; }
        }
    }
}
