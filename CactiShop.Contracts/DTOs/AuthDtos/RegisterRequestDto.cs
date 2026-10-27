namespace CactiShop.Contracts.DTOs.AuthDtos; 
public record RegisterRequestDto(string Name, string Password, string ConfirmPassword);