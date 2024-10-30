using Microsoft.AspNetCore.Mvc;

namespace WebAPI
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly ICosmosDbService _cosmosDbService;

        public MessagesController(ICosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllMessages() => Ok(await _cosmosDbService.GetAllMessagesAsync());

        [HttpGet("valueRange")]
        public async Task<IActionResult> GetMessagesByValueRange([FromQuery] double min, [FromQuery] double max)
        {
            return Ok(await _cosmosDbService.GetMessagesByValueRangeAsync(min, max));
        }

        [HttpGet("dateRange")]
        public async Task<IActionResult> GetMessagesByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            return Ok(await _cosmosDbService.GetMessagesByDateRangeAsync(startDate, endDate));
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetTotalMessageCount() => Ok(await _cosmosDbService.GetTotalMessageCountAsync());

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics() => Ok(await _cosmosDbService.GetStatisticsAsync());
    }

}
