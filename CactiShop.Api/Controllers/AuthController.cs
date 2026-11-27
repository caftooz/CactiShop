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
public class AuthController(AppDbContext context) : ControllerBase
{
    [HttpPost(ApiRoutes.Auth.Register)]
    public async Task<IActionResult> Register(RegisterRequestDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Login) || dto.Login.Length > 64)
            return BadRequest(new { message = "Логин обязателен и должен быть не длиннее 64 символов." });
        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8)
            return BadRequest(new { message = "Пароль должен содержать не менее 8 символов." });
        if (dto.Password != dto.ConfirmPassword)
            return BadRequest(new { message = "Пароли не совпадают." });

        var login = dto.Login.Trim();
        if (await context.Users.AnyAsync(u => u.Name.ToLower() == login.ToLower(), ct))
            return Conflict(new { message = "Пользователь с таким именем уже существует." });

        var roleId = await context.Roles.Where(r => r.Name == "User")
            .Select(r => (int?)r.Id).FirstOrDefaultAsync(ct);
        if (roleId is null)
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: "Пользовательская роль не настроена.");

        var (hash, salt) = Encryptor.HashPassword(dto.Password);
        context.Users.Add(new User { Name = login, RoleId = roleId.Value, PasswordHash = hash, PasswordSalt = salt });
        await context.SaveChangesAsync(ct);
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpPost(ApiRoutes.Auth.Login)]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto, TokenService tokens, CancellationToken ct)
    {
        var user = await context.Users.Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Name == dto.Login, ct);

        if (user is null || !Encryptor.Verify(dto.Password, user.PasswordHash, user.PasswordSalt))
            return Unauthorized(new { message = "Неправильный логин или пароль." });

        if (Encryptor.NeedsUpgrade(user.PasswordSalt))
        {
            (user.PasswordHash, user.PasswordSalt) = Encryptor.HashPassword(dto.Password);
            await context.SaveChangesAsync(ct);
        }

        return Ok(new AuthResponseDto(
            new UserInfoDto(user.Name, RoleService.GetUserRole(user.Role.Name)),
            tokens.CreateToken(user)));
    }

    [HttpGet(ApiRoutes.Auth.Me)]
    [Authorize]
    public async Task<ActionResult<UserInfoDto>> Me(CancellationToken ct)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var user = await context.Users.Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId, ct);
        return user is null
            ? Unauthorized()
            : Ok(new UserInfoDto(user.Name, RoleService.GetUserRole(user.Role.Name)));
    }
}
