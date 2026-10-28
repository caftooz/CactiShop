using CactiShop.Desktop.Models;
using CactiShop.Desktop.Services.Interfaces;
using CactiShop.Contracts.Auth;

namespace CactiShop.Desktop.Services;

public class SessionService : ISessionService
{
    public AppUser? CurrentUser { get; private set; }

    public string? Token { get; private set; }

    public bool IsLoggedIn => CurrentUser is not null;

    public bool IsAdmin => CurrentUser?.Role == UserRole.Admin;

    public event EventHandler? CurrentUserChanged;

    public void SignIn(AppUser user, string token)
    {
        CurrentUser = user;
        Token = token;
        CurrentUserChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SignOut()
    {
        if (CurrentUser is null)
            return;

        CurrentUser = null;
        Token = null;
        CurrentUserChanged?.Invoke(this, EventArgs.Empty);
    }
}
