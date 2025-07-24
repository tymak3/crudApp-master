using crudApp.Services.AutomationService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace crudApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutomationsController : ControllerBase
    {
        private readonly IAutomationService _automationService;
        public AutomationsController(IAutomationService automationService)
        {
            _automationService = automationService;
        }
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            try
            {
                var result = await _automationService.RunAutomation();
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("players")]
        public async Task<IActionResult> GetPlayers()
        {
            try
            {
                var players = await _automationService.GetPlayerStats();
                return Ok(players);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}