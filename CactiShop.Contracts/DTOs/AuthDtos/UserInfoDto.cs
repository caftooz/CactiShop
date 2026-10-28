namespace CactiShop.Contracts.DTOs.AuthDtos;

public record UserInfoDto(string Login, UserRole Role);

public enum UserRole
{
    Admin,
    User
}