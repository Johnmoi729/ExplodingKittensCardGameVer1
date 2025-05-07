using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ExplodingKittens.API.Hubs
{
    /// <summary>
    /// SignalR hub for real-time game communications
    /// </summary>
    [Authorize]
    public class GameHub : Hub
    {
        public async Task JoinGameGroup(string gameId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        }

        public async Task LeaveGameGroup(string gameId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
        }

        // Methods that will be called by the server to notify clients
        // These will be implemented in Week 2 as part of the game logic
    }
}