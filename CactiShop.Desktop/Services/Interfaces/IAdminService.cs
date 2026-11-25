using CactiShop.Contracts.Catalog;
using CactiShop.Contracts.OrderPoints;
using CactiShop.Contracts.Orders;
using CactiShop.Desktop.Models;

namespace CactiShop.Desktop.Services.Interfaces;

internal interface IAdminService
{
    Task<IReadOnlyList<Order>> GetAllOrdersAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ProductReviewListDto>> GetAllReviewsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<OrderStatusDto>> GetOrderStatusesAsync(CancellationToken ct = default);

    Task<object> UpdateProductAsync(ProductSummary dto, CancellationToken ct = default);
    Task<object> DeleteProductAsync(int id, CancellationToken ct = default);

    Task<object> UpdateCategoryAsync(Category category, CancellationToken ct = default); //TODO
    Task<object> DeleteCategoryAsync(int categoryId, CancellationToken ct = default); //TODO

    Task<object> UpdateOrderAsync(Order order, CancellationToken ct = default); //TODO
    Task<object> DeleteOrderAsync(int orderId, CancellationToken ct = default); //TODO

    Task<object> UpdateOrderPointAsync(OrderPointDto orderPoint, CancellationToken ct = default); //TODO
    Task<object> DeleteOrderPointAsync(int orderPointId, CancellationToken ct = default); //TODO

    Task<object> DeleteReviewAsync(int reviewId, CancellationToken ct = default); //TODO
}
