using CactiShop.Contracts.DTOs.AuthDtos;

namespace CactiShop.Api.Services
{
    public class RoleService
    {
        public static UserRole GetUserRole(string roleName)
        {
            return roleName switch
            {
                "Admin" => UserRole.Admin,
                "User" => UserRole.User,
                _ => throw new ArgumentException($"Invalid role name: {roleName}")
            };
        }
    }
}
