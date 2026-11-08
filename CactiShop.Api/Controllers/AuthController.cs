using System.Security.Claims;
using CactiShop.Api.Data;
using CactiShop.Api.Data.Models;
using CactiShop.Api.Services;
using CactiShop.Contracts;
using CactiShop.Contracts.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CactiShop.Api.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost(ApiRoutes.Auth.Register)]
    public async Task<ActionResult> Register(RegisterRequestDto dto)
    {
        if (dto.Password != dto.ConfirmPassword) return BadRequest("Пароли не совпадают");
        if (await _context.Users.AnyAsync(u => u.Name == dto.Login))
            return BadRequest("Пользователь с таким именем уже существует!");

        var roleId = _context.Roles.FirstOrDefault(r => r.Name == "User")?.Id ?? -1;
        if (roleId == -1) return NotFound("Пользовательская роль отсутствует");

        (string hash, string salt) = Encryptor.HashPassword(dto.Password);

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

    [HttpPost(ApiRoutes.Auth.Login)]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto, TokenService tokens)
    {
        var user = await _context.Users.Include(u => u.Role)
                                       .FirstOrDefaultAsync(u => u.Name == dto.Login);

        if (user == null) return Unauthorized("Неправильный логин или пароль");

        if (Encryptor.Verify(dto.Password, user.PasswordHash, user.PasswordSalt))
        {
            return Ok(new AuthResponseDto(new UserInfoDto(user.Name, RoleService.GetUserRole(user.Role.Name)), tokens.CreateToken(user)));
        }

        return Unauthorized("Неправильный логин или пароль");
    }

    [HttpGet(ApiRoutes.Auth.Me)]
    [Authorize]
    public async Task<ActionResult<UserInfoDto>> Me()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId);
        return user == null ? Unauthorized() : Ok(new UserInfoDto(user.Name, RoleService.GetUserRole(user.Role.Name)));
    }
}
