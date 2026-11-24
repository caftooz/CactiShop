using CactiShop.Contracts;
using CactiShop.Contracts.Orders;
using CactiShop.Desktop.Models;
using CactiShop.Desktop.Services.Interfaces;

namespace CactiShop.Desktop.Services.Api;

internal class OrderService(ApiClient _apiClient) : IOrderService
{
    public async Task<IReadOnlyList<Order>> GetUserOrdersAsync(bool? byDescending, CancellationToken ct = default)
    {
        var dtos = await _apiClient.GetAsync<List<OrderDto>>(ApiRoutes.Orders.OrdersUser, ct);
        return dtos.Select(d => new Order(d.Id, d.Status, d.OrderPoint, d.DeliveryAddress, d.Date, d.TotalPrice, d.OrderProducts.Select(op => new OrderProduct(op.ProductName, op.ProductPrice, op.ProductQuantity)))).ToList();
    }
    public async Task<IReadOnlyList<Order>> GetAllOrdersAsync(CancellationToken ct = default)
    {
        var dtos = await _apiClient.GetAsync<List<OrderDto>>(ApiRoutes.Orders.AllOrders, ct);
        return dtos.Select(d => new Order(d.Id, d.Status, d.OrderPoint, d.DeliveryAddress, d.Date, d.TotalPrice, d.OrderProducts.Select(op => new OrderProduct(op.ProductName, op.ProductPrice, op.ProductQuantity)))).ToList();
    }
    public async Task<object> ChangeOrderStatusAsync(int orderdId, CancellationToken ct = default) =>
        await _apiClient.PatchAsync<object>(ApiRoutes.Orders.OrderById(orderdId), ct);
    public async Task<int> CreateOrderAsync(CreateOrderDto dto, CancellationToken ct = default) =>
        await _apiClient.PostAsync<int>(ApiRoutes.Orders.CreateOrder, ct);
}
