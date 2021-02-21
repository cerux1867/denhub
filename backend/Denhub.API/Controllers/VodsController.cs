using System.Threading.Tasks;
using Denhub.API.Models;
using Denhub.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Denhub.API.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class VodsController : ControllerBase {
        private readonly IVodsService _vodsService;

        public VodsController(IVodsService vodsService) {
            _vodsService = vodsService;
        }
        
        [HttpGet("{channelName}")]
        public async Task<IActionResult> GetAllByChannelNameAsync([FromRoute] string channelName, [FromQuery] VodsParameters vodsParameters) {
            var channelId = await _vodsService.GetChannelIdByChannelNameAsync(channelName);
            if (channelId == -1) {
                return NotFound();
            }

            var vods = await _vodsService.GetAllByIdAsync(channelId, vodsParameters.Page, vodsParameters.Limit);
            return Ok(vods);
        }
    }
}