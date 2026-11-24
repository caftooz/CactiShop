using CactiShop.Api.Data;
using CactiShop.Api.Data.Models;
using CactiShop.Contracts;
using CactiShop.Contracts.Catalog;
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
}
