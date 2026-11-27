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

    public static class Orders
    {
        public const string OrdersUser = "api/orders";
        public static string OrderById(int orderdId) => $"{OrdersUser}/{orderdId}";
        public const string CreateOrder = "api/orders";

        public const string GetOrderPoints = "api/orders/order-points";
    }

    public static class Admin
    {
        public const string AllOrders = "api/admin/orders/all";
        public const string AddProduct = "api/admin/catalog";
        public const string UploadImage = "api/admin/upload-image";
        public const string AllReviews = "api/admin/reviews";
        public const string AllOrderStatuses = "api/admin/order-statuses";
        public const string UpdateProduct = "api/admin/product";
        public const string DeleteProduct = "api/admin/product";
        public const string UpdateCategory = "api/admin/category";
        public const string DeleteCategory = "api/admin/category";
        public const string UpdateOrder = "api/admin/orders";
        public const string DeleteOrder = "api/admin/orders";
        public const string UpdateOrderPoint = "api/admin/order-point";
        public const string DeleteOrderPoint = "api/admin/order-point";
        public const string DeleteReview = "api/admin/reviews";
    }
}
