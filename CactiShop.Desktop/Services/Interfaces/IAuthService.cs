namespace CactiShop.Desktop.Services.Interfaces;

public interface IAuthService
{
    Task LoginAsync(string login, string password, CancellationToken ct = default);
    Task RegisterAsync(string login, string password, string confirmPassword, CancellationToken ct = default);
    void Logout();
}
