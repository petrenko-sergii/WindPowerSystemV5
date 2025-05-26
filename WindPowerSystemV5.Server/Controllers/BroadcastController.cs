using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace WindPowerSystemV5.Server.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class BroadcastController : ControllerBase
{
    private IHubContext<HealthCheckHub> _hub;

    public BroadcastController(IHubContext<HealthCheckHub> hub)
    {
        _hub = hub;
    }

    [HttpGet]
    public async Task<IActionResult> Update()
    {
        await _hub.Clients.All.SendAsync("Update", "test-message from BroadcastController");

        return Ok("Update message sent from BroadcastController.");
    }
}
