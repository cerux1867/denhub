using System;
using System.Threading.Tasks;
using Denhub.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Denhub.API.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class LogsController : ControllerBase {
        private readonly ILogsService _logsService;

        public LogsController(ILogsService service) {
            _logsService = service;
        }

        [HttpGet("{channelId:long}/logs")]
        public async Task<IActionResult> GetByChannelId([FromRoute] long channelId, [FromQuery] DateTime timestamp,
            [FromQuery] string cursor, [FromQuery] int limit = 100) {
            var result = await _logsService.GetByChannelIdAsync(channelId, timestamp, cursor, limit);

            return Ok(result);
        }
    }
}