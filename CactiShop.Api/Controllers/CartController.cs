using System.Security.Claims;
using CactiShop.Api.Data;
using CactiShop.Api.Data.Models;
using CactiShop.Contracts;
using CactiShop.Contracts.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CactiShop.Api.Controllers;

[ApiController]
[Authorize]
public sealed class CartController(AppDbContext context) : ControllerBase
{
    [HttpPatch(ApiRoutes.Cart.Items)]
    public async Task<IActionResult> ChangeProductInCart(QuantityCartDto dto, CancellationToken ct)
    {
        if (dto.ProductId <= 0 || dto.NewQuantity < 0)
            return BadRequest(new { message = "Товар и количество указаны некорректно." });

        if (!TryGetUserId(out var userId))
            return Unauthorized();

        if (dto.NewQuantity == 0)
            return await DeleteCartProduct(dto.ProductId, ct);

        var product = await context.Products
            .FirstOrDefaultAsync(product => product.Id == dto.ProductId && product.IsActive, ct);
        if (product is null)
            return NotFound(new { message = "Товар не найден или недоступен." });

        if (dto.NewQuantity > product.Quantity)
            return BadRequest(new { message = "Недостаточно товара на складе." });

        var cartItem = await context.CartItems
            .FirstOrDefaultAsync(item => item.ProductId == dto.ProductId && item.UserId == userId, ct);

        if (cartItem is null)
        {
            context.CartItems.Add(new CartItem
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Quantity = dto.NewQuantity
            });
        }
        else
        {
            cartItem.Quantity = dto.NewQuantity;
        }

        await context.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpDelete(ApiRoutes.Cart.ItemById)]
    public async Task<IActionResult> DeleteCartProduct(int productId, CancellationToken ct = default)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var cartItem = await context.CartItems
            .FirstOrDefaultAsync(item => item.ProductId == productId && item.UserId == userId, ct);

        if (cartItem is null)
            return NotFound(new { message = "Товара нет в корзине." });

        context.CartItems.Remove(cartItem);
        await context.SaveChangesAsync(ct);
        return NoContent();
    }

    private bool TryGetUserId(out int userId) =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
}
