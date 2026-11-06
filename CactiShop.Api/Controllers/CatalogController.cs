using CactiShop.Api.Data;
using CactiShop.Api.Data.Models;
using CactiShop.Contracts.Catalog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace CactiShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CatalogController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCatalog()
        {
            var result = await _context.Products
                .Where(p => p.IsActive)
                .Include(p => p.Category)
                .Select(p => ToListDto(p))
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound(new { message = "Товар не найден" });

            return Ok(new ProductDetailDto(ToListDto(product), product.Description));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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

        [HttpGet("filter")]
        public async Task<IActionResult> SearchProduct(ProductQuery dto)
        {
            var querySearch = _context.Products.Where(p => dto.Query == null ? true : p.Name.Contains(dto.Query, StringComparison.InvariantCultureIgnoreCase));
            var queryPrice = querySearch.Where(p => dto.MinPrice == null ? true : p.Price > dto.MinPrice 
            && dto.MaxPrice == null ? true : p.Price < dto.MaxPrice);
            var queryCategory = queryPrice.Where(p => dto.CategoryId == null ? true : dto.CategoryId == p.CategoryId);
            IEnumerable<ProductListItemDto> p = queryCategory.Select(ToListDto);
            return Ok(p);
        }

        private ProductListItemDto ToListDto(Product p)=> 
            new ProductListItemDto(
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Category.Name,
                    p.ImagePath,
                    p.AvgRate,
                    p.Quantity);
    }
}
