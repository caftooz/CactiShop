namespace CactiShop.Contracts.DTOs.AuthDtos; 
public record RegisterRequestDto(string Login, string Password, string ConfirmPassword);