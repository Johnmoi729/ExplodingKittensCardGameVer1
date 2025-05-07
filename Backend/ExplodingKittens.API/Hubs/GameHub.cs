using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ExplodingKittens.Application.DTOs;

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

        // Methods that will be called by clients

        /// <summary>
        /// Notify when a player plays a card
        /// </summary>
        public async Task NotifyCardPlayed(string gameId, string playerId, string cardName)
        {
            await Clients.Group(gameId).SendAsync("CardPlayed", new { PlayerId = playerId, CardName = cardName });
        }

        /// <summary>
        /// Notify when a player draws a card
        /// </summary>
        public async Task NotifyCardDrawn(string gameId, string playerId)
        {
            await Clients.Group(gameId).SendAsync("CardDrawn", new { PlayerId = playerId });
        }

        /// <summary>
        /// Notify when a player explodes
        /// </summary>
        public async Task NotifyPlayerExploded(string gameId, string playerId)
        {
            await Clients.Group(gameId).SendAsync("PlayerExploded", new { PlayerId = playerId });
        }

        /// <summary>
        /// Notify when it's a player's turn
        /// </summary>
        public async Task NotifyPlayerTurn(string gameId, string playerId)
        {
            await Clients.Group(gameId).SendAsync("PlayerTurn", new { PlayerId = playerId });
        }

        /// <summary>
        /// Notify when the game ends
        /// </summary>
        public async Task NotifyGameEnded(string gameId, string winnerId)
        {
            await Clients.Group(gameId).SendAsync("GameEnded", new { WinnerId = winnerId });
        }
    }
}