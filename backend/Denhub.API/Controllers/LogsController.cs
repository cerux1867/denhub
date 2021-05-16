using System;
using System.Threading.Tasks;
using Denhub.API.Models;
using Denhub.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Denhub.API.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class LogsController : ControllerBase {
        private readonly ILogsService _logsService;

        public LogsController(ILogsService service) {
            _logsService = service;
        }

        /// <summary>
        /// Retrieves logs from the specified channel
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /logs/38746172?limit=20
        /// 
        /// </remarks>
        /// <param name="channelId">Twitch channel ID of the channel</param>
        /// <param name="timestamp">Optional starting timestamp</param>
        /// <param name="cursor">Optional pagination cursor to continue a previous paged query</param>
        /// <param name="limit">Optional pagination limit</param>
        /// <returns>A list of messages and a pagination cursor if required</returns>
        /// <response code="200">
        /// Returns a list of messages with an optional pagination cursor if one is returned
        /// </response>
        [HttpGet("{channelId:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<TwitchChatMessagesResult>> GetByChannelIdAsync([FromRoute] long channelId,
            [FromQuery] DateTime timestamp,
            [FromQuery] string cursor, [FromQuery] int limit = 100) {
            var result = await _logsService.GetByChannelIdAsync(channelId, timestamp, cursor, limit);

            return Ok(result);
        }
    }
}