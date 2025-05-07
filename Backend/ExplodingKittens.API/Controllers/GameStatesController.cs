using System;
using System.Threading.Tasks;
using ExplodingKittens.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExplodingKittens.API.Controllers
{
    [ApiController]
    [Route("api/games/{gameId}/state")]
    [Authorize]
    public class GameStatesController : ControllerBase
    {
        private readonly IGameStateService _gameStateService;

        public GameStatesController(IGameStateService gameStateService)
        {
            _gameStateService = gameStateService;
        }

        [HttpGet]
        public async Task<IActionResult> GetGameState(string gameId)
        {
            try
            {
                var gameState = await _gameStateService.GetGameStateAsync(gameId);
                return Ok(gameState);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}