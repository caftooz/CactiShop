namespace CactiShop.Api.Data.Models;

public class User
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public string Name { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string PasswordSalt { get; set; } = "";

    public Role Role { get; set; } = null!;
    public ICollection<CartItems> CartItems { get; set; } = [];
    public ICollection<Order> Orders { get; set; } = [];
}

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public class OrderStatus
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public class OrderPoint
{
    public int Id { get; set; }
    public string Address { get; set; } = "";
}

public class Product
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string ImagePath { get; set; } = "";
    public double AvgRate { get; set; }
    public bool IsActive { get; set; } = true;

    public Category Category { get; set; } = null!;
    public ICollection<ProductReview> ProductReviews { get; set; } = [];
    public ICollection<Order> Orders { get; set; } = [];
}

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int StatusId { get; set; }
    public int? OrderPointId { get; set; }
    public string? DeliveryAddress { get; set; }
    public DateOnly Date { get; set; }
    public decimal TotalPrice { get; set; }

    public User User { get; set; } = null!;
    public OrderStatus Status { get; set; } = null!;
    public OrderPoint? OrderPoint { get; set; }
    public ICollection<OrderProducts> OrderProducts { get; set; } = [];
}

public class OrderProducts
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public decimal ProductPrice { get; set; }
    public int ProductQuantity { get; set; }

    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
}

public class ProductReview
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = "";
    public string Text { get; set; } = "";
    public int Rate { get; set; }

    public Product Product { get; set; } = null!;
    public User User { get; set; } = null!;
}

public class CartItems
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }

    public User User { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
