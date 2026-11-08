using CactiShop.Contracts.Auth;

namespace CactiShop.Desktop.Models;

public record AppUser(string Login, UserRole Role);
