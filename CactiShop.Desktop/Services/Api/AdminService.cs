using System.Net.Http;
using CactiShop.Contracts;
using CactiShop.Contracts.Catalog;
using CactiShop.Contracts.OrderPoints;
using CactiShop.Contracts.Orders;
using CactiShop.Desktop.Models;
using CactiShop.Desktop.Services.Interfaces;

namespace CactiShop.Desktop.Services.Api;

internal sealed class AdminService(ApiClient apiClient) : IAdminService
{
    public async Task<IReadOnlyList<Order>> GetAllOrdersAsync(CancellationToken ct = default)
    {
        var dtos = await apiClient.GetAsync<List<OrderDto>>(ApiRoutes.Admin.AllOrders, ct);
        return dtos.Select(dto => new Order(
            dto.Id, dto.Status, dto.OrderPoint, dto.DeliveryAddress, dto.Date, dto.TotalPrice,
            dto.OrderProducts.Select(item =>
                new OrderProduct(item.ProductName, item.ProductPrice, item.ProductQuantity)).ToList()))
            .ToList();
    }

    public Task<IReadOnlyList<ProductReviewListDto>> GetAllReviewsAsync(CancellationToken ct = default) =>
        ReadListAsync<ProductReviewListDto>(ApiRoutes.Admin.AllReviews, ct);

    public Task<IReadOnlyList<OrderStatusDto>> GetOrderStatusesAsync(CancellationToken ct = default) =>
        ReadListAsync<OrderStatusDto>(ApiRoutes.Admin.AllOrderStatuses, ct);

    public async Task<object> UpdateProductAsync(UpdateProductDto dto, CancellationToken ct = default)
    {
        await apiClient.SendAsync(HttpMethod.Post, ApiRoutes.Admin.UpdateProduct, dto, ct);
        return true;
    }

    public async Task<object> DeleteProductAsync(int productId, CancellationToken ct = default)
    {
        await apiClient.SendAsync(HttpMethod.Delete, $"{ApiRoutes.Admin.DeleteProduct}?id={productId}", ct: ct);
        return true;
    }

    public async Task<object> UpdateCategoryAsync(Category category, CancellationToken ct = default)
    {
        await apiClient.SendAsync(HttpMethod.Put, ApiRoutes.Admin.UpdateCategory, category, ct);
        return true;
    }

    public async Task<object> DeleteCategoryAsync(int categoryId, CancellationToken ct = default)
    {
        await apiClient.SendAsync(HttpMethod.Delete, $"{ApiRoutes.Admin.DeleteCategory}?categoryId={categoryId}", ct: ct);
        return true;
    }

    public async Task<object> UpdateOrderAsync(Order order, CancellationToken ct = default)
    {
        await apiClient.SendAsync(HttpMethod.Put, ApiRoutes.Admin.UpdateOrder, order, ct);
        return true;
    }

    public async Task<object> DeleteOrderAsync(int orderId, CancellationToken ct = default)
    {
        await apiClient.SendAsync(HttpMethod.Delete, $"{ApiRoutes.Admin.DeleteOrder}?orderId={orderId}", ct: ct);
        return true;
    }

    public async Task<object> UpdateOrderPointAsync(OrderPointDto orderPoint, CancellationToken ct = default)
    {
        await apiClient.SendAsync(HttpMethod.Put, ApiRoutes.Admin.UpdateOrderPoint, orderPoint, ct);
        return true;
    }

    public async Task<object> DeleteOrderPointAsync(int orderPointId, CancellationToken ct = default)
    {
        await apiClient.SendAsync(HttpMethod.Delete, $"{ApiRoutes.Admin.DeleteOrderPoint}?orderPointId={orderPointId}", ct: ct);
        return true;
    }

    public async Task<object> DeleteReviewAsync(int reviewId, CancellationToken ct = default)
    {
        await apiClient.SendAsync(HttpMethod.Delete, $"{ApiRoutes.Admin.DeleteReview}?reviewId={reviewId}", ct: ct);
        return true;
    }

    private async Task<IReadOnlyList<T>> ReadListAsync<T>(string route, CancellationToken ct) =>
        await apiClient.GetAsync<List<T>>(route, ct);
}
