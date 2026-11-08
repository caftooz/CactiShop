namespace CactiShop.Contracts;

public static class ApiRoutes
{
    public static class Auth
    {
        public const string Register = "api/auth/register";
        public const string Login = "api/auth/login";
        public const string Me = "api/auth/me";
    }

    public static class Catalog
    {
        public const string List = "api/catalog";
        public const string ById = List + "/{id:int}";
        public static string ByIdUrl(int id) => $"{List}/{id}";
    }

    public static class Categories
    {
        public const string List = "api/categories";
    }

    public static class Cart
    {
        public const string Items = "api/cart";
        public const string ItemById = Items + "/{productId:int}";
        public static string ItemByIdUrl(int productId) => $"{Items}/{productId}";
    }
}
