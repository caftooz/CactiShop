using CactiShop.Contracts;
using CactiShop.Contracts.Catalog;
using CactiShop.Contracts.OrderPoints;
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

    public async Task<object> UpdateProductAsync(UpdateProductDto productSummaryDto, CancellationToken ct = default) =>
        apiClient.PostAsync<UpdateProductDto>(ApiRoutes.Admin.UpdateProduct, productSummaryDto, ct);
    public async Task<object> DeleteProductAsync(int productId, CancellationToken ct = default) =>
        apiClient.PostAsync<ProductSummary>(ApiRoutes.Admin.DeleteProduct, productId, ct);
    public async Task<object> UpdateCategoryAsync(Category category, CancellationToken ct = default) =>
        apiClient.PostAsync<ProductSummary>(ApiRoutes.Admin.UpdateCategory, category, ct);
    public async Task<object> DeleteCategoryAsync(int categoryId, CancellationToken ct = default) =>
        apiClient.PostAsync<ProductSummary>(ApiRoutes.Admin.DeleteCategory, categoryId, ct);
    public async Task<object> UpdateOrderAsync(Order order, CancellationToken ct = default) =>
        apiClient.PostAsync<ProductSummary>(ApiRoutes.Admin.UpdateOrder, order, ct);
    public async Task<object> DeleteOrderAsync(int orderId, CancellationToken ct = default) =>
        apiClient.PostAsync<ProductSummary>(ApiRoutes.Admin.DeleteOrder, orderId, ct);
    public async Task<object> UpdateOrderPointAsync(OrderPointDto orderPoint, CancellationToken ct = default) =>
        apiClient.PostAsync<ProductSummary>(ApiRoutes.Admin.UpdateOrderPoint, orderPoint, ct);
    public async Task<object> DeleteOrderPointAsync(int orderPointId, CancellationToken ct = default) =>
        apiClient.PostAsync<ProductSummary>(ApiRoutes.Admin.DeleteOrderPoint, orderPointId, ct);
    public async Task<object> DeleteReviewAsync(int reviewId, CancellationToken ct = default) =>
        apiClient.PostAsync<ProductSummary>(ApiRoutes.Admin.DeleteReview, reviewId, ct);
}
