using CactiShop.Contracts;
using CactiShop.Contracts.Catalog;
using CactiShop.Contracts.Orders;
using CactiShop.Desktop.Models;
using CactiShop.Desktop.Services.Interfaces;

namespace CactiShop.Desktop.Services.Api;

internal class AdminService(ApiClient apiClient) : IAdminService
{
    public async Task<IReadOnlyList<Order>> GetAllOrdersAsync(CancellationToken ct = default)
    {
        var dtos = await apiClient.GetAsync<List<OrderDto>>(ApiRoutes.Admin.AllOrders, ct);
        return dtos.Select(d => new Order(d.Id, d.Status, d.OrderPoint, d.DeliveryAddress, d.Date, d.TotalPrice, d.OrderProducts.Select(op => new OrderProduct(op.ProductName, op.ProductPrice, op.ProductQuantity)))).ToList();
    }
    public async Task<IReadOnlyList<ProductReviewListDto>> GetAllReviewsAsync(CancellationToken ct = default)
    {
        var dtos = await apiClient.GetAsync<List<ProductReviewListDto>>(ApiRoutes.Admin.AllReviews, ct);
        return dtos;
    }

    public async Task<IReadOnlyList<OrderStatusDto>> GetOrderStatusesAsync(CancellationToken ct = default)
    {
        var dtos = await apiClient.GetAsync<List<OrderStatusDto>>(ApiRoutes.Admin.AllOrderStatuses, ct);
        return dtos;
    }
}
