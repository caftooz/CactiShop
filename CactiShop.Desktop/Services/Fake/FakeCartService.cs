using CactiShop.Desktop.Services.Interfaces;

namespace CactiShop.Desktop.Services.Fake;

public class FakeCartService : ICartService
{
    private Dictionary<int, int> _cartItems = new();

    public Task<int> GetProductQuantity(int productId, CancellationToken ct = default)
    {
        _cartItems.TryGetValue(productId, out int quantity);
        return Task.FromResult(quantity);
    }

    public Task SetProductQuantity(int productId, int quantity, CancellationToken ct = default)
    {
        if (quantity <= 0)
        {
            _cartItems.Remove(productId);
        }
        else
        {
            _cartItems[productId] = quantity;
        }
        return Task.CompletedTask;
    }
}
