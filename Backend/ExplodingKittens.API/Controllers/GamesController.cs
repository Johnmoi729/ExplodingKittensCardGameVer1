using System;
using System.Threading.Tasks;
using ExplodingKittens.Application.DTOs;
using ExplodingKittens.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExplodingKittens.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GamesController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveGames()
        {
            try
            {
                var games = await _gameService.GetActiveGamesAsync();
                return Ok(games);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGame(string id)
        {
            try
            {
                var game = await _gameService.GetGameByIdAsync(id);
                return Ok(game);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("player/{playerId}")]
        public async Task<IActionResult> GetGamesByPlayerId(string playerId)
        {
            try
            {
                var games = await _gameService.GetGamesByPlayerIdAsync(playerId);
                return Ok(games);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateGame([FromBody] CreateGameDto createGameDto)
        {
            try
            {
                var game = await _gameService.CreateGameAsync(createGameDto);
                return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/join")]
        public async Task<IActionResult> JoinGame(string id, [FromBody] string playerId)
        {
            try
            {
                var result = await _gameService.JoinGameAsync(id, playerId);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/start")]
        public async Task<IActionResult> StartGame(string id)
        {
            try
            {
                var result = await _gameService.StartGameAsync(id);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}