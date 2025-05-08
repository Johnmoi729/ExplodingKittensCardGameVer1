using System.Threading.Tasks;
using ExplodingKittens.Application.DTOs;
using ExplodingKittens.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExplodingKittens.API.Controllers
{
    [ApiController]
    [Route("api/games/{gameId}/status")]
    [Authorize]
    public class GameStatusController : ControllerBase
    {
        private readonly IGameActionService _gameActionService;

        public GameStatusController(IGameActionService gameActionService)
        {
            _gameActionService = gameActionService;
        }

        [HttpGet("{playerId}")]
        public async Task<ActionResult<GameStatusDto>> GetPlayerGameStatus(string gameId, string playerId)
        {
            var status = await _gameActionService.GetGameStatusAsync(gameId, playerId);
            return Ok(status);
        }
    }
}