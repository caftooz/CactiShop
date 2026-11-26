using CactiShop.Contracts;
using CactiShop.Contracts.Cart;
using CactiShop.Desktop.Services.Interfaces;

namespace CactiShop.Desktop.Services.Api;

internal class CartService(ApiClient _apiClient) : ICartService
{
    private Dictionary<int, int>? _cartItems;
    private readonly SemaphoreSlim _lock = new(1, 1);

    private async Task RefreshCartItemsAsync(CancellationToken ct = default)
    {
        var response = await _apiClient.GetAsync<List<CartProductQuantityDto>>(ApiRoutes.Cart.Items, ct).ConfigureAwait(false);
        var dict = new Dictionary<int, int>();
        foreach (var cpqDto in response)
        {
            dict[cpqDto.ProductId] = cpqDto.Quantity;
        }
        _cartItems = dict;
    }

    public async Task<int> GetProductQuantity(int productId, CancellationToken ct = default)
    {
        if (_cartItems is null)
        {
            await _lock.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                if (_cartItems is null)
                    await RefreshCartItemsAsync(ct).ConfigureAwait(false);
            }
            finally
            {
                _lock.Release();
            }
        }
        return _cartItems.GetValueOrDefault(productId, 0);
    }

    public async Task SetProductQuantity(int productId, int quantity, CancellationToken ct = default)
    {
        await _apiClient.PatchAsync<object>(ApiRoutes.Cart.Items, new SetCartProductQuantityDto(productId, quantity), ct).ConfigureAwait(false);
        await RefreshCartItemsAsync(ct).ConfigureAwait(false);
    }
}
