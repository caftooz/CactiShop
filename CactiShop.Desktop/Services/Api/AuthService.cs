using System.Net.Http;
using CactiShop.Contracts;
using CactiShop.Contracts.Auth;
using CactiShop.Desktop.Models;
using CactiShop.Desktop.Services.Interfaces;

namespace CactiShop.Desktop.Services.Api;

public class AuthService(ApiClient _apiClient, ISessionService _session) : IAuthService
{
    public async Task LoginAsync(string login, string password, CancellationToken ct = default)
    {
        var r = await _apiClient.PostAsync<AuthResponseDto>(ApiRoutes.Auth.Login, new LoginDto(login, password), ct);
        _session.SignIn(new AppUser(r.Info.Login, r.Info.Role), r.AuthToken);
    }

    public Task RegisterAsync(string login, string password, string confirmPassword, CancellationToken ct = default) =>
        _apiClient.SendAsync(HttpMethod.Post, ApiRoutes.Auth.Register,
            new RegisterRequestDto(login, password, confirmPassword), ct);

    public void Logout() => _session.SignOut();
}
