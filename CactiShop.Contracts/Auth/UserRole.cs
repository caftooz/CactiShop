using System.Text.Json.Serialization;

namespace CactiShop.Contracts.Auth;

/// <summary>
///     Роль вошедшего. Гость - это отсутствие входа, отдельной роли у него нет.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRole
{
    User,
    Admin
}
