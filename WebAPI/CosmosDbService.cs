using Microsoft.Azure.Cosmos;

namespace WebAPI
{
    public class CosmosDbService(CosmosClient cosmosClient, string databaseId, string containerId) : ICosmosDbService
    {
        private readonly Container _container = cosmosClient.GetContainer(databaseId, containerId);

        public async Task<IEnumerable<Message>> GetAllMessages()
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

        public async Task<IEnumerable<Message>> GetMessagesByValueRange(double min, double max)
        {
            var query = $"SELECT * FROM c WHERE c.Value >= {min} AND c.Value <= {max}";
            var iterator = _container.GetItemQueryIterator<Message>(query);
            var results = new List<Message>();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response);
            }
            return results;
        }

        public async Task<IEnumerable<Message>> GetMessagesByDateRange(DateTime startDate, DateTime endDate)
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

        public async Task<int> GetTotalMessageCount()
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

        public async Task<string> GetStatistics()
        {
            var avgQuery = "SELECT VALUE AVG(c.Value) FROM c";
            var avgIterator = _container.GetItemQueryIterator<double>(avgQuery);
            double avg = 0;

            if (avgIterator.HasMoreResults)
            {
                var avgResponse = await avgIterator.ReadNextAsync();
                avg = avgResponse.FirstOrDefault();
            }

            var minQuery = "SELECT VALUE MIN(c.Value) FROM c";
            var minIterator = _container.GetItemQueryIterator<double>(minQuery);
            double min = 0;

            if (minIterator.HasMoreResults)
            {
                var minResponse = await minIterator.ReadNextAsync();
                min = minResponse.FirstOrDefault();
            }

            var maxQuery = "SELECT VALUE MAX(c.Value) FROM c";
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
