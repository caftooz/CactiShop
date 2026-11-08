namespace CactiShop.Contracts.Auth;

public record AuthResponseDto(UserInfoDto Info, string AuthToken);
