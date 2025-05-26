using Microsoft.AspNetCore.SignalR;

namespace WindPowerSystemV5.Server;

public class HealthCheckHub : Hub
{
    public async Task ClientUpdate(string message) =>
        await Clients.All.SendAsync("ClientUpdate", message);
}
