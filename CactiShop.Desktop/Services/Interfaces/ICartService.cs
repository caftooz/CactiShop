namespace CactiShop.Desktop.Services.Interfaces;

public interface ICartService
{
    Task SetProductQuantity(int productId, int quantity, CancellationToken ct);
}
