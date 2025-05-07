using System;
using System.Threading.Tasks;
using ExplodingKittens.API.Hubs;
using ExplodingKittens.Application.DTOs;
using ExplodingKittens.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ExplodingKittens.API.Controllers
{
    [ApiController]
    [Route("api/games/{gameId}/actions")]
    [Authorize]
    public class GameActionsController : ControllerBase
    {
        private readonly IGameActionService _gameActionService;
        private readonly IHubContext<GameHub> _hubContext;

        public GameActionsController(
            IGameActionService gameActionService,
            IHubContext<GameHub> hubContext)
        {
            _gameActionService = gameActionService;
            _hubContext = hubContext;
        }

        [HttpPost("play-card")]
        public async Task<IActionResult> PlayCard(string gameId, [FromBody] PlayCardDto playCardDto)
        {
            try
            {
                var result = await _gameActionService.PlayCardAsync(gameId, playCardDto);

                // Notify all players in the game
                await _hubContext.Clients.Group(gameId).SendAsync("GameStateUpdated", result);

                // If it's a see future card, only send the data to the player who played it
                if (playCardDto.CardId.StartsWith("see_future"))
                {
                    var seeFutureResult = await _gameActionService.SeeFutureAsync(gameId, playCardDto.PlayerId);
                    await _hubContext.Clients.User(playCardDto.PlayerId).SendAsync("SeeFutureResult", seeFutureResult);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("draw-card")]
        public async Task<IActionResult> DrawCard(string gameId, [FromBody] DrawCardDto drawCardDto)
        {
            try
            {
                var result = await _gameActionService.DrawCardAsync(gameId, drawCardDto.PlayerId);

                // Notify all players in the game
                await _hubContext.Clients.Group(gameId).SendAsync("GameStateUpdated", result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("play-combo")]
        public async Task<IActionResult> PlayCombo(string gameId, [FromBody] PlayComboDto playComboDto)
        {
            try
            {
                var result = await _gameActionService.PlayComboAsync(gameId, playComboDto);

                // Notify all players in the game
                await _hubContext.Clients.Group(gameId).SendAsync("GameStateUpdated", result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("defuse-kitten")]
        public async Task<IActionResult> DefuseKitten(string gameId, [FromBody] DefuseKittenDto defuseKittenDto)
        {
            try
            {
                var result = await _gameActionService.DefuseKittenAsync(gameId, defuseKittenDto);

                // Notify all players in the game
                await _hubContext.Clients.Group(gameId).SendAsync("GameStateUpdated", result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}