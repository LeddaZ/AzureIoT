using Microsoft.Azure.Cosmos;

namespace WebAPI
{
    public class CosmosDbService : ICosmosDbService
    {
        private readonly Container _container;

        public CosmosDbService(CosmosClient cosmosClient, string databaseId, string containerId)
        {
            _container = cosmosClient.GetContainer(databaseId, containerId);
        }

        public async Task<IEnumerable<Message>> GetAllMessagesAsync()
        {
            var query = "SELECT * FROM c";
            var iterator = _container.GetItemQueryIterator<Message>(query);
            var results = new List<Message>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response);
            }
            return results;
        }

        public async Task<IEnumerable<Message>> GetMessagesByValueRangeAsync(double min, double max)
        {
            var query = $"SELECT * FROM c WHERE c.MessageContent >= {min} AND c.MessageContent <= {max}";
            var iterator = _container.GetItemQueryIterator<Message>(query);
            var results = new List<Message>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response);
            }
            return results;
        }

        public async Task<IEnumerable<Message>> GetMessagesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var query = $"SELECT * FROM c WHERE c.Timestamp >= '{startDate:O}' AND c.Timestamp <= '{endDate:O}'";
            var iterator = _container.GetItemQueryIterator<Message>(query);
            var results = new List<Message>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response);
            }
            return results;
        }

        public async Task<int> GetTotalMessageCountAsync()
        {
            var query = "SELECT VALUE COUNT(1) FROM c";
            var iterator = _container.GetItemQueryIterator<int>(query);

            if (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                return response.FirstOrDefault();
            }
            return 0;
        }

        public async Task<string> GetStatisticsAsync()
        {
            var avgQuery = "SELECT VALUE AVG(c.MessageContent) FROM c";
            var avgIterator = _container.GetItemQueryIterator<double>(avgQuery);
            double avg = 0;

            if (avgIterator.HasMoreResults)
            {
                var avgResponse = await avgIterator.ReadNextAsync();
                avg = avgResponse.FirstOrDefault();
            }

            var minQuery = "SELECT VALUE MIN(c.MessageContent) FROM c";
            var minIterator = _container.GetItemQueryIterator<double>(minQuery);
            double min = 0;

            if (minIterator.HasMoreResults)
            {
                var minResponse = await minIterator.ReadNextAsync();
                min = minResponse.FirstOrDefault();
            }

            var maxQuery = "SELECT VALUE MAX(c.MessageContent) FROM c";
            var maxIterator = _container.GetItemQueryIterator<double>(maxQuery);
            double max = 0;

            if (maxIterator.HasMoreResults)
            {
                var maxResponse = await maxIterator.ReadNextAsync();
                max = maxResponse.FirstOrDefault();
            }

            return ("Avg: " + avg + ", Min: " + min + ", Max: " + max);
        }
    }

}
