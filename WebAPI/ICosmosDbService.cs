namespace WebAPI
{
    public interface ICosmosDbService
    {
        Task<IEnumerable<Message>> GetAllMessages();
        Task<IEnumerable<Message>> GetMessagesByValueRange(double min, double max);
        Task<IEnumerable<Message>> GetMessagesByDateRange(DateTime startDate, DateTime endDate);
        Task<int> GetTotalMessageCount();
        Task<string> GetStatistics();
    }
}
