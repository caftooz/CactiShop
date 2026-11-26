using CactiShop.Api.Data;
using CactiShop.Api.Data.Models;
using CactiShop.Contracts;
using CactiShop.Contracts.Catalog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CactiShop.Api.Controllers;

[ApiController]
public class CatalogController : ControllerBase
{
    private readonly AppDbContext _context;
    public CatalogController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet(ApiRoutes.Catalog.List)]
    public async Task<ActionResult<List<ProductListItemDto>>> GetCatalog([FromQuery] ProductQuery q)
    {
        var query = _context.Products.Where(p => p.IsActive);

        if (!string.IsNullOrWhiteSpace(q.Query))
            query = query.Where(p => EF.Functions.ILike(p.Name, $"%{q.Query.Trim()}%"));
        if (q.CategoryId is { } c) query = query.Where(p => p.CategoryId == c);
        if (q.MinPrice is { } min) query = query.Where(p => p.Price >= min);
        if (q.MaxPrice is { } max) query = query.Where(p => p.Price <= max);

        return await query.Select(p => new ProductListItemDto(
            p.Id, p.Name, p.Price, p.Category.Name, p.ImagePath, p.AvgRate, p.Quantity))
            .ToListAsync();
    }

    [HttpGet(ApiRoutes.Catalog.ById)]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductReviews)
            .ThenInclude(pr => pr.User.Name).FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound(new { message = "Товар не найден" });

        return Ok(new ProductDetailsDto(
            ToListDto(product),
            product.Description,
            product.ProductReviews.Select(pr => new ProductReviewListDto(pr.Id, pr.Text, pr.User.Name, product.Name)
        ).ToList()));
    }

    [HttpGet(ApiRoutes.Categories.List)]
    public async Task<ActionResult<List<CategoryDto>>> GetCategories()
    {
        var categories = await _context.Categories.Select(c => new CategoryDto(c.Id, c.Name)).ToListAsync();
        return Ok(categories);
    }

    private static ProductListItemDto ToListDto(Product p) =>
        new ProductListItemDto(
                p.Id,
                p.Name,
                p.Price,
                p.Category.Name,
                p.ImagePath,
                p.AvgRate,
                p.Quantity);
}
