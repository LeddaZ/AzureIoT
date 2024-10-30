namespace WebAPI
{
    public interface ICosmosDbService
    {
        Task<IEnumerable<Message>> GetAllMessagesAsync();
        Task<IEnumerable<Message>> GetMessagesByValueRangeAsync(double min, double max);
        Task<IEnumerable<Message>> GetMessagesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<int> GetTotalMessageCountAsync();
        Task<string> GetStatisticsAsync();
    }
}
