using CactiShop.Contracts.Auth;
using CactiShop.Desktop.Models;
using CactiShop.Desktop.Services.Api;
using CactiShop.Desktop.Services.Interfaces;

namespace CactiShop.Desktop.Services.Fake;

public class FakeAuthService(ISessionService _sessionService) : IAuthService
{
    private readonly Dictionary<string, (string Password, UserRole Role)> _users = new()
    {
        ["admin"] = ("admin", UserRole.Admin),
        ["user"] = ("user", UserRole.User),
    };
    public async Task LoginAsync(string login, string password, CancellationToken ct = default)
    {
        await Task.Delay(300, ct);
        if (!_users.TryGetValue(login, out var u) || u.Password != password)
            throw new ApiException(401, "Неправильный логин или пароль");

        _sessionService.SignIn(new AppUser(login, u.Role), "fake-token");
    }

    public async Task RegisterAsync(string login, string password, string confirmPassword, CancellationToken ct = default)
    {
        await Task.Delay(300, ct);
        if (password != confirmPassword) throw new ApiException(400, "Пароли не совпадают");
        if (!_users.TryAdd(login, (password, UserRole.User)))
            throw new ApiException(400, "Пользователь с таким именем уже существует");
    }

    public void Logout() => _sessionService.SignOut();
}
