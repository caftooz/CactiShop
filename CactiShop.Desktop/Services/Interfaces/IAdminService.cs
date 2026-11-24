using CactiShop.Contracts.Catalog;
using CactiShop.Contracts.Orders;
using CactiShop.Desktop.Models;

namespace CactiShop.Desktop.Services.Interfaces;

internal interface IAdminService
{
    Task<IReadOnlyList<Order>> GetAllOrdersAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ProductReviewListDto>> GetAllReviewsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<OrderStatusDto>> GetOrderStatusesAsync(CancellationToken ct = default);

    Task<object> UpdateProduct(ProductSummary dto, CancellationToken ct = default);
    Task<object> DeleteProduct(int id, CancellationToken ct = default);
}
