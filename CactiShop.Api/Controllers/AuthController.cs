using CactiShop.Api.Data;
using CactiShop.Api.Data.Models;
using CactiShop.Api.Services;
using CactiShop.Contracts.DTOs.AuthDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CactiShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AuthController(AppDbContext context) {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterRequestDto dto) {
            if (dto.Password != dto.ConfirmPassword) return BadRequest("Пароли не совпадают");
            if (await _context.Users.AnyAsync(u => u.Name == dto.Login)) 
                return BadRequest("Пользователь с таким именем уже существует!");

            var roleId = _context.Roles.FirstOrDefault(r => r.Name == "User")?.Id ?? -1;
            if (roleId == -1) return NotFound("Пользовательская роль отсутствует");

            (string salt, string hash) = Encryptor.HashPassword(dto.Password);

            var user = new User
            {
                Name = dto.Login,
                RoleId = roleId,
                PasswordSalt = salt,
                PasswordHash = hash
            };
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto, TokenService tokens) {
            var user = await _context.Users.FirstAsync(u => u.Name == dto.Login);

            if (user == null) return Unauthorized("Неправильный логин или пароль");

            if (Encryptor.Verify(dto.Password, user.PasswordHash, user.PasswordSalt)) return Ok(new AuthResponseDto(tokens.CreateToken(user)));

            return Unauthorized("Неправильный логин или пароль");
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserInfoDto>> Me() {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId);
            return user == null ? Unauthorized() : Ok(new UserInfoDto(user.Name, RoleService.GetUserRole(user.Role.Name)));
        }
    }
}
