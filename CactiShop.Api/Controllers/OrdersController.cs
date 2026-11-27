using System.Security.Claims;
using CactiShop.Api.Data;
using CactiShop.Api.Data.Models;
using CactiShop.Contracts;
using CactiShop.Contracts.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CactiShop.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public sealed class OrdersController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetUserOrders(
        [FromQuery] bool? descending = true,
        CancellationToken ct = default)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var query = context.Orders
            .AsNoTracking()
            .Where(order => order.UserId == userId)
            .Include(order => order.Status)
            .Include(order => order.OrderPoint)
            .Include(order => order.OrderProducts)
                .ThenInclude(item => item.Product);

        query = descending == false
            ? query.OrderBy(order => order.Date).ThenBy(order => order.Id)
            : query.OrderByDescending(order => order.Date).ThenByDescending(order => order.Id);

        var orders = await query.ToListAsync(ct);
        return Ok(orders.Select(ToDto).ToList());
    }

    [HttpGet(ApiRoutes.Orders.GetOrderPoints)]
    public async Task<ActionResult<IReadOnlyList<CactiShop.Contracts.OrderPoints.OrderPointDto>>> GetOrderPoints(CancellationToken ct)
    {
        var points = await context.OrderPoints
            .AsNoTracking()
            .OrderBy(point => point.Address)
            .Select(point => new CactiShop.Contracts.OrderPoints.OrderPointDto(point.Id, point.Address))
            .ToListAsync(ct);
        return Ok(points);
    }

    [HttpPost]
    public async Task<ActionResult<int>> CreateOrder(CreateOrderDto dto, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var requested = dto.OrderProducts?
            .GroupBy(item => item.ProductId)
            .Select(group => new { ProductId = group.Key, Quantity = group.Sum(item => item.ProductQuantity) })
            .ToList();

        if (requested is null || requested.Count == 0)
            return BadRequest(new { message = "Заказ не может быть пустым." });

        if (requested.Any(item => item.ProductId <= 0 || item.Quantity <= 0))
            return BadRequest(new { message = "Товары и их количество должны быть положительными." });

        if (dto.OrderPointId is null && string.IsNullOrWhiteSpace(dto.DeliveryAddress))
            return BadRequest(new { message = "Укажите пункт выдачи или адрес доставки." });

        if (dto.OrderPointId is { } pointId &&
            !await context.OrderPoints.AnyAsync(point => point.Id == pointId, ct))
            return BadRequest(new { message = "Пункт выдачи не найден." });

        var productIds = requested.Select(item => item.ProductId).ToArray();
        var products = await context.Products
            .Where(product => productIds.Contains(product.Id))
            .ToDictionaryAsync(product => product.Id, ct);

        if (products.Count != requested.Count)
            return BadRequest(new { message = "Один или несколько товаров не найдены." });

        foreach (var item in requested)
        {
            var product = products[item.ProductId];
            if (!product.IsActive)
                return BadRequest(new { message = $"Товар «{product.Name}» недоступен." });
            if (product.Quantity < item.Quantity)
                return BadRequest(new { message = $"Недостаточно товара «{product.Name}» на складе." });
        }

        var order = new Order
        {
            UserId = userId,
            OrderPointId = dto.OrderPointId,
            DeliveryAddress = string.IsNullOrWhiteSpace(dto.DeliveryAddress) ? null : dto.DeliveryAddress.Trim(),
            StatusId = 1,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            OrderProducts = requested.Select(item =>
            {
                var product = products[item.ProductId];
                product.Quantity -= item.Quantity;
                return new OrderProduct
                {
                    ProductId = product.Id,
                    ProductQuantity = item.Quantity,
                    ProductPrice = product.Price
                };
            }).ToList()
        };

        context.Orders.Add(order);
        await context.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetUserOrders), new { }, order.Id);
    }

    [HttpPatch("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdvanceOrderStatus(int id, CancellationToken ct)
    {
        var order = await context.Orders.FirstOrDefaultAsync(order => order.Id == id, ct);
        if (order is null)
            return NotFound(new { message = "Заказ не найден." });

        // Statuses are seeded with IDs 1..6. The final state is not advanced.
        if (order.StatusId >= 1 && order.StatusId < 5)
            order.StatusId++;

        await context.SaveChangesAsync(ct);
        return NoContent();
    }

    private bool TryGetUserId(out int userId) =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);

    private static OrderDto ToDto(Order order) =>
        new(
            order.Id,
            order.Status.Name,
            order.OrderPoint?.Address,
            order.DeliveryAddress,
            order.Date,
            order.TotalPrice,
            order.OrderProducts.Select(item =>
                new OrderProductsDto(item.Product.Name, item.ProductPrice, item.ProductQuantity)).ToList());
}
