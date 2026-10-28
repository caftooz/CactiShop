namespace CactiShop.Contracts.Auth; 
public record RegisterRequestDto(string Login, string Password, string ConfirmPassword);