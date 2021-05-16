using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Denhub.API.Models;
using Denhub.API.Results;
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
        /// Retrieves logs from the specified channel by the channel name
        /// </summary>
        /// <remarks>
        /// Sample request
        /// 
        ///     GET /logs?channelName=esfandtv
        /// 
        /// </remarks>
        /// <param name="channelName">Twitch name of the channel</param>
        /// <param name="timestamp">Optional starting timestamp</param>
        /// <param name="cursor">Optional pagination cursor to continue a previous paged query</param>
        /// <param name="limit">Optional pagination limit</param>
        /// <param name="order">Defines sorting order. Allowed values are 'asc' and 'desc'. Default is 'asc'.</param>
        /// <returns>A list of messages and a pagination cursor if required</returns>
        /// <response code="200">
        /// Returns a list of messages with an optional pagination cursor if one is returned
        /// </response>
        /// <response code="404">
        /// Channel with the given name was not found
        /// </response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TwitchChatMessagesResult>> GetByChannelNameAsync(
            [Required] [FromQuery] string channelName, [FromQuery] DateTime timestamp,
            [FromQuery] string cursor, [FromQuery] int limit = 100, [FromQuery] string order = "asc") {
            var result = await _logsService.GetByChannelAsync(channelName, timestamp, order, cursor, limit);
            if (result.Type == ResultType.NotFound) {
                return NotFound(result.Errors.First());
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Retrieves logs from the specified channel by the channel ID
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
        /// <param name="order">Defines sorting order. Allowed values are 'asc' and 'desc'. Default is 'asc'.</param>
        /// <returns>A list of messages and a pagination cursor if required</returns>
        /// <response code="200">
        /// Returns a list of messages with an optional pagination cursor if one is returned
        /// </response>
        [HttpGet("{channelId:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<TwitchChatMessagesResult>> GetByChannelIdAsync([FromRoute] long channelId,
            [FromQuery] DateTime timestamp,
            [FromQuery] string cursor, [FromQuery] int limit = 100, [FromQuery] string order = "asc") {
            var result = await _logsService.GetByChannelAsync(channelId, timestamp, order, cursor, limit);

            return Ok(result.Value);
        }
    }
}