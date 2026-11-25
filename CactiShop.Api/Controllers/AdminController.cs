using CactiShop.Api.Data;
using CactiShop.Api.Data.Models;
using CactiShop.Contracts;
using CactiShop.Contracts.Catalog;
using CactiShop.Contracts.OrderPoints;
using CactiShop.Contracts.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CactiShop.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private AppDbContext _context;
    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet(ApiRoutes.Admin.AllOrders)]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = _context.Orders.Include(o => o.OrderProducts).ToList();
        return Ok(orders);
    }

    [HttpGet(ApiRoutes.Admin.AllReviews)]
    public async Task<IActionResult> GetReviews()
    {
        var reviews = await _context.ProductReviews
            .Include(pr => pr.User.Name)
            .Include(pr => pr.Product.Name)
            .Select(pr => new ProductReviewListDto(pr.Id, pr.Text, pr.User.Name, pr.Product.Name))
            .ToListAsync();

        return Ok(reviews);
    }

    [HttpGet(ApiRoutes.Admin.AllOrderStatuses)]
    public async Task<IActionResult> GetStatuses()
    {
        var statuses = await _context.OrderStatuses.ToListAsync();
        return Ok(statuses);
    }

    [HttpPost(ApiRoutes.Admin.AddProduct)]
    public async Task<IActionResult> AddProduct(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            CategoryId = dto.CategoryId,
            Description = dto.Description,
            Price = dto.Price,
            Quantity = dto.Quantity,
            ImagePath = dto.ImagePath,
            IsActive = dto.IsActive
        };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return Created();
    }

    [HttpPost(ApiRoutes.Admin.UploadImage)]
    public async Task<IActionResult> UploadImage(IFormFile image)
    {
        if (image == null || image.Length == 0) return BadRequest(new { message = "Файл не выбран" });

        var uploadsFolder = Path.Combine(AppContext.BaseDirectory, "wwwroot/image");
        if (!Directory.Exists(uploadsFolder)) return BadRequest(new { message = $"Мамка не найдена: {uploadsFolder}" });

        var uniqueFilename = Guid.NewGuid().ToString() + Path.GetExtension(uploadsFolder);
        var filepath = Path.Combine(uploadsFolder, uniqueFilename);

        using (var stream = new FileStream(filepath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }

        return Ok(new { fileName = uniqueFilename });
    }

    [HttpPost(ApiRoutes.Admin.UpdateProduct)]
    public async Task<IActionResult> UpdateProduct(UpdateProductDto dto)
    {
        var product = await _context.Products.FindAsync(dto.Id);
        if (product == null) return BadRequest(new { message = "Продукт не найден" });
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == dto.CategoryName);
        if (category == null) return BadRequest(new { message = $"Категория {dto.CategoryName} не найдена" });
        product.Name = dto.Name;
        product.Category = category;
        product.Price = dto.Price;
        product.ImagePath = dto.ImageUrl ?? "";
        product.Description = dto.Description;
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete(ApiRoutes.Admin.DeleteProduct)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return BadRequest(new { message = "Продукт не найден" });
        _context.Remove(product);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut(ApiRoutes.Admin.UpdateCategory)]
    public async Task<IActionResult> UpdateCategory(Category category, CancellationToken ct = default)
    {
        var existingCategory = await _context.Categories.FindAsync(new object[] { category.Id }, ct);
        if (existingCategory == null) return BadRequest(new { message = "Категория не найдена" });

        existingCategory.Name = category.Name;
        await _context.SaveChangesAsync(ct);
        return Ok();
    }

    [HttpDelete(ApiRoutes.Admin.DeleteCategory)]
    public async Task<IActionResult> DeleteCategory(int categoryId, CancellationToken ct = default)
    {
        var category = await _context.Categories.FindAsync(new object[] { categoryId }, ct);
        if (category == null) return BadRequest(new { message = "Категория не найдена" });

        _context.Remove(category);
        await _context.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpPut(ApiRoutes.Admin.UpdateOrder)]
    public async Task<IActionResult> UpdateOrder(OrderDto order, CancellationToken ct = default)
    {
        var existingOrder = await _context.Orders
            .Include(o => o.OrderProducts)
            .FirstOrDefaultAsync(o => o.Id == order.Id, ct);

        if (existingOrder == null) return BadRequest(new { message = "Заказ не найден" });

        var status = await _context.OrderStatuses.FirstOrDefaultAsync(os => os.Name == order.Status);
        if (status == null) return BadRequest(new { message = "Неизвестный передаваемый статус" });
        existingOrder.Status = status;

        var orderPoint = await _context.OrderPoints.FirstOrDefaultAsync(op => op.Address == order.OrderPoint);
        if (orderPoint == null) return BadRequest(new { message = "Неизвестный пункт выдачи" });
        existingOrder.OrderPoint = orderPoint;

        existingOrder.DeliveryAddress = order.DeliveryAddress;

        await _context.SaveChangesAsync(ct);
        return Ok();
    }

    [HttpDelete(ApiRoutes.Admin.DeleteOrder)]
    public async Task<IActionResult> DeleteOrder(int orderId, CancellationToken ct = default)
    {
        var order = await _context.Orders.FindAsync(orderId, ct);
        if (order == null) return BadRequest(new { message = "Заказ не найден" });

        _context.Remove(order);
        await _context.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpPut(ApiRoutes.Admin.UpdateOrderPoint)]
    public async Task<IActionResult> UpdateOrderPoint(OrderPointDto orderPoint, CancellationToken ct = default)
    {
        var existingOrderPoint = await _context.OrderPoints.FindAsync(orderPoint.Id, ct);
        if (existingOrderPoint == null) return BadRequest(new { message = "Пункт выдачи не найден" });

        existingOrderPoint.Address = orderPoint.Address;
        await _context.SaveChangesAsync(ct);
        return Ok();
    }

    [HttpDelete(ApiRoutes.Admin.DeleteOrderPoint)]
    public async Task<IActionResult> DeleteOrderPoint(int orderPointId, CancellationToken ct = default)
    {
        var orderPoint = await _context.OrderPoints.FindAsync(orderPointId, ct);
        if (orderPoint == null) return BadRequest(new { message = "Пункт выдачи не найден" });

        _context.Remove(orderPoint);
        await _context.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete(ApiRoutes.Admin.DeleteReview)]
    public async Task<IActionResult> DeleteReview(int reviewId, CancellationToken ct = default)
    {
        var review = await _context.ProductReviews.FindAsync(reviewId, ct);
        if (review == null) return BadRequest(new { message = "Отзыв не найден" });

        _context.Remove(review);
        await _context.SaveChangesAsync(ct);
        return NoContent();
    }
}
