using System.Net.Http;
using CactiShop.Contracts;
using CactiShop.Contracts.OrderPoints;
using CactiShop.Contracts.Orders;
using CactiShop.Desktop.Models;
using CactiShop.Desktop.Services.Interfaces;

namespace CactiShop.Desktop.Services.Api;

internal sealed class OrderService(ApiClient apiClient) : IOrderService
{
    public async Task<IReadOnlyList<Order>> GetUserOrdersAsync(bool? byDescending, CancellationToken ct = default)
    {
        var url = ApiRoutes.Orders.OrdersUser;
        if (byDescending.HasValue)
            url += $"?descending={byDescending.Value.ToString().ToLowerInvariant()}";

        var dtos = await apiClient.GetAsync<List<OrderDto>>(url, ct);
        return dtos.Select(dto => new Order(
            dto.Id,
            dto.Status,
            dto.OrderPoint,
            dto.DeliveryAddress,
            dto.Date,
            dto.TotalPrice,
            dto.OrderProducts.Select(item =>
                new OrderProduct(item.ProductName, item.ProductPrice, item.ProductQuantity)).ToList()))
            .ToList();
    }

    public async Task<IReadOnlyList<OrderPointDto>> GetOrderPointsAsync(CancellationToken ct = default) =>
        await apiClient.GetAsync<List<OrderPointDto>>(ApiRoutes.Orders.GetOrderPoints, ct);

    public async Task<object> ChangeOrderStatusAsync(int orderId, CancellationToken ct = default)
    {
        await apiClient.SendAsync(HttpMethod.Patch, ApiRoutes.Orders.OrderById(orderId), cancellationToken: ct);
        return true;
    }

    public async Task<int> CreateOrderAsync(CreateOrderDto dto, CancellationToken ct = default) =>
        await apiClient.PostAsync<int>(ApiRoutes.Orders.CreateOrder, dto, ct);
}
