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
public class CartController : ControllerBase
{
    private readonly AppDbContext _context;
    public CartController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPatch(ApiRoutes.Cart.Items)]
    public async Task<IActionResult> ChangeProductInCart(QuantityCartDto dto)
    {
        if (dto.NewQuantity == 0)
            return await DeleteCartProduct(dto.ProductId);

        if (dto.NewQuantity < 0)
            return BadRequest(new { message = "Количество товара в корзине не может быть меньше 1" });

        int userId = GetUserId();
        CartItems? cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.ProductId == dto.ProductId && ci.UserId == userId);
        if (cartItem != null)
        {
            cartItem.Quantity = dto.NewQuantity;
            await _context.SaveChangesAsync();
            return Ok();
        }

        if (!await _context.Users.AnyAsync(u => u.Id == userId))
            return BadRequest(new { message = "Пользователь не найден" });
        if (!await _context.Products.AnyAsync(p => p.Id == dto.ProductId))
            return BadRequest(new { message = "Товар не найден" });

        _context.CartItems.Add(new CartItems
        {
            UserId = userId,
            ProductId = dto.ProductId,
            Quantity = dto.NewQuantity
        });

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete(ApiRoutes.Cart.ItemById)]
    public async Task<IActionResult> DeleteCartProduct(int id)
    {
        var userId = GetUserId();

        var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.ProductId == id && ci.UserId == userId);

        if (cartItem == null)
            return BadRequest(new { message = "Товара нет в корзине" });

        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize]
    public int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
