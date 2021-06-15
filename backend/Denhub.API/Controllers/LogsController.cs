using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Denhub.API.Models;
using Denhub.API.Results;
using Denhub.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

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
        /// <param name="parameters">Query parameters</param>
        /// <returns>A list of messages and a pagination cursor if required</returns>
        /// <response code="200">
        /// Returns a list of messages with an optional pagination cursor if one is returned
        /// </response>
        /// <response code="404">
        /// Channel was not found or there are no stored logs from this channel
        /// </response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult>> GetAllAsync([Required][FromQuery] string channelName, [FromQuery] LogsParameters parameters) {
            var result =
                await _logsService.GetByChannelNameAsync(channelName, parameters.StartDate, parameters.EndDate, parameters.Order == "desc" ? SortDirection.Descending : SortDirection.Ascending, parameters.Cursor, parameters.Limit, parameters.Username);
            if (result.Type == ResultType.NotFound) {
                return NotFound(new { error = result.Errors.First() });
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Retrieves logs from the specified channel by the channel ID
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     GET /channels/38746172/logs?limit=20
        /// 
        /// </remarks>
        /// <param name="channelId">Twitch channel ID of the channel</param>
        /// <param name="parameters">Query parameters</param>
        /// <returns>A list of messages and a pagination cursor if required</returns>
        /// <response code="200">
        /// Returns a list of messages with an optional pagination cursor if one is returned
        /// </response>
        /// <response code="404">
        /// Channel or user were not found or there are no stored logs from this channel/user
        /// </response>
        [HttpGet("~/Channels/{channelId:long}/Logs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PagedResult>> GetByChannelIdAsync([FromRoute] long channelId, [FromQuery] LogsParameters parameters) {
            var result = await _logsService.GetByChannelIdAsync(channelId, parameters.StartDate, parameters.EndDate, parameters.Order == "desc" ? SortDirection.Descending : SortDirection.Ascending, parameters.Cursor, parameters.Limit, parameters.Username);
            if (result.Type == ResultType.NotFound) {
                return NotFound(new { error = result.Errors.First() });
            }

            return Ok(result.Value);
        }
    }
}