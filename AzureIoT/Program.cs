using System.Text;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Cosmos;
using Azure.Messaging;

namespace AzureIoT
{
    public class MessageRecord
    {
        public required string id { get; set; }
        public required string MessaggioID { get; set; }
        public required string DeviceId { get; set; }
        public required double MessageContent { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Received { get; set; }
    }

    class Program
    {
        private static readonly string HostName = Environment.GetEnvironmentVariable("HOST_NAME") ?? "";
        private static readonly string DeviceId = Environment.GetEnvironmentVariable("DEVICE_ID") ?? "";
        private static readonly string AccessKey = Environment.GetEnvironmentVariable("ACCESS_KEY") ?? "";
        private static readonly string connectionString = "HostName="+HostName+";SharedAccessKeyName=iothubowner;SharedAccessKey="+AccessKey;
        private static readonly string cosmosEndpointUri = Environment.GetEnvironmentVariable("COSMOS_ENDPOINT") ?? "";
        private static readonly string cosmosPrimaryKey = Environment.GetEnvironmentVariable("COSMOS_KEY") ?? "";
        private static readonly string cosmosDatabase = Environment.GetEnvironmentVariable("DB_NAME") ?? "";
        private static readonly string cosmosContainerId = Environment.GetEnvironmentVariable("CONTAINER_ID") ?? "";

        static async Task Main(string[] args)
        {
            var serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            var cosmosClient = new CosmosClient(cosmosEndpointUri, cosmosPrimaryKey);
            var database = cosmosClient.GetDatabase(cosmosDatabase);
            var container = database.GetContainer(cosmosContainerId);

            Console.Write("Enter a number: ");
            double num = Convert.ToDouble(Console.ReadLine());

            try
            {
                await SendCloudToDeviceMessageAsync(serviceClient, num.ToString());
                Console.WriteLine("Message sent to device!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while sending message to device: {ex.Message}");
            }

            try
            {
                await SaveMessageToCosmosDBAsync(container, num.ToString());
                Console.WriteLine("Message saved to CosmosDB!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while saving message to CosmosDB: {ex.Message}");
            }
        }

        private static async Task SendCloudToDeviceMessageAsync(ServiceClient serviceClient, string messageContent)
        {
            var message = new Message(Encoding.ASCII.GetBytes(messageContent));
            message.Properties.Add("MessageType", "Command");
            message.Properties.Add("MessageId", Guid.NewGuid().ToString());
            message.Properties.Add("DeviceId", DeviceId);
            message.Properties.Add("Timestamp", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            message.Properties.Add("Received", "false");

            await serviceClient.SendAsync(DeviceId, message);
        }

        private static async Task SaveMessageToCosmosDBAsync(Container container, string messageContent)
        {
            var messageRecord = new MessageRecord
            {
                id = Guid.NewGuid().ToString(),
                MessaggioID = Guid.NewGuid().ToString(),
                DeviceId = DeviceId,
                MessageContent = Convert.ToDouble(messageContent),
                Timestamp = DateTime.UtcNow,
                Received = false
            };

            await container.CreateItemAsync(messageRecord, new PartitionKey(messageRecord.MessaggioID));
        }
    }
}
