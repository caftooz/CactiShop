using System.Security.Claims;
using CactiShop.Api.Data;
using CactiShop.Api.Data.Models;
using CactiShop.Contracts.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CactiShop.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _context;
    public OrdersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserOrders([FromBody] bool? byDesc)
    {
        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var ordersQuery = _context.Orders.Include(o => o.OrderProducts).Where(p => p.Id == userId);
        if (byDesc == null) return Ok(ordersQuery.Select(ToDto));
        ordersQuery = byDesc == true ? ordersQuery.OrderByDescending(o => o.Date) : ordersQuery.OrderBy(o => o.Date);
        return Ok(ordersQuery.Select(ToDto));
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
    {
        if (dto.OrderProducts is null || !dto.OrderProducts.Any())
            return BadRequest("Заказ не может быть пустым");

        if (dto.OrderProducts.Any(p => p.ProductQuantity <= 0))
            return BadRequest("Количество должно быть больше нуля");

        var items = dto.OrderProducts
            .GroupBy(p => p.ProductId)
            .Select(g => new { ProductId = g.Key, Quantity = g.Sum(x => x.ProductQuantity) })
            .ToList();

        var productIds = dto.OrderProducts.Select(i => i.ProductId).ToList();

        var prices = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p.Price);

        var order = new Order
        {
            OrderPointId = dto.OrderPointId,
            DeliveryAddress = dto.DeliveryAddress,
            UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            StatusId = 1,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            OrderProducts = items.Select(op => new OrderProduct
            {
                ProductId = op.ProductId,
                ProductQuantity = op.Quantity,
                ProductPrice = prices[op.ProductId]
            }).ToList()
        };
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return Ok(order.Id);
    }
    [HttpPatch("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ChangeOrderStatus(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) { return NotFound(); }

        int sId = order.StatusId;

        order.StatusId = sId == 1 ? 2 : sId == 2 ? 1 : 1;

        await _context.SaveChangesAsync();
        return Ok();
    }

    private OrderDto ToDto(Order o) =>
        new OrderDto(
            o.Id,
            o.Status.Name,
            o.OrderPoint?.Address,
            o.DeliveryAddress,
            o.Date,
            o.TotalPrice,
            o.OrderProducts.Select(op =>
            new OrderProductsDto(op.Product.Name, op.ProductPrice, op.ProductQuantity))
            );
}
