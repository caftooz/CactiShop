using CactiShop.Desktop.Services.Interfaces;

namespace CactiShop.Desktop.Services.Fake;

public class FakeCartService : ICartService
{
    private Dictionary<int, int> _cartItems = new();

    public async Task<int> GetProductQuantity(int productId, CancellationToken ct = default)
    {
        await Task.Delay(400, ct);
        _cartItems.TryGetValue(productId, out int quantity);
        return quantity;
    }

    public async Task SetProductQuantity(int productId, int quantity, CancellationToken ct = default)
    {
        await Task.Delay(400, ct);
        if (quantity <= 0)
        {
            _cartItems.Remove(productId);
        }
        else
        {
            _cartItems[productId] = quantity;
        }
    }
}
