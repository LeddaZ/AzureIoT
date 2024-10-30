using Microsoft.AspNetCore.Mvc;

namespace WebAPI
{
    [ApiController]
    [Route("api/messages")]
    public class MessagesController(ICosmosDbService cosmosDbService) : ControllerBase
    {
        [HttpGet("all")]
        public async Task<IActionResult> GetAllMessages() =>
            Ok(await cosmosDbService.GetAllMessages());

        [HttpGet("valueRange")]
        public async Task<IActionResult> GetMessagesByValueRange(
            [FromQuery] double min,
            [FromQuery] double max
        )
        {
            return Ok(await cosmosDbService.GetMessagesByValueRange(min, max));
        }

        [HttpGet("dateRange")]
        public async Task<IActionResult> GetMessagesByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate
        )
        {
            return Ok(await cosmosDbService.GetMessagesByDateRange(startDate, endDate));
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetTotalMessageCount() =>
            Ok(await cosmosDbService.GetTotalMessageCount());

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics() =>
            Ok(await cosmosDbService.GetStatistics());
    }
}
