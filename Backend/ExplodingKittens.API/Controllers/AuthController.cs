using System;
using System.Threading.Tasks;
using ExplodingKittens.Application.DTOs;
using ExplodingKittens.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExplodingKittens.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var result = await _authService.RegisterAsync(registerDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var token = await _authService.LoginAsync(loginDto);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}