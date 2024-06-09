using Common.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace TaxiWebAPI.Hubs
{
    [Authorize]
    public class RideHub : Hub<IRideChat>
    {
        public async Task SendMessage(string group, string user, string message)
        {
            await Clients.Group(group).SendMessage(user, message);
        }

        public async Task AddDriver()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "drivers");
        }
    }
}
