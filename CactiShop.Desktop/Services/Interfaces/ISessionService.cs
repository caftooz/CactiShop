using CactiShop.Desktop.Models;

namespace CactiShop.Desktop.Services.Interfaces;

public interface ISessionService
{
    AppUser? CurrentUser { get; }

    string? Token { get; }

    bool IsLoggedIn { get; }
    bool IsAdmin { get; }

    event EventHandler? CurrentUserChanged;

    void SignIn(AppUser user, string token);
    void SignOut();
}
