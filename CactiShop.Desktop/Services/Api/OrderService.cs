using CactiShop.Contracts.Orders;
using CactiShop.Desktop.Services.Interfaces;

namespace CactiShop.Desktop.Services.Api;

public class OrderService(ApiClient _apiClient) : IOrderService
{
    public async Task<IReadOnlyList<OrderDto>> GetUserOrdersAsync(bool? byDescending, CancellationToken ct = default) =>
    await _apiClient.GetAsync<List<OrderDto>>("api/orders", ct);

    public async Task<IReadOnlyList<OrderDto>> GetAllOrdersAsync(CancellationToken ct = default) =>
        await _apiClient.GetAsync<List<OrderDto>>("api/orders/all", ct);
    public async Task<object> ChangeOrderStatusAsync(int orderdId, CancellationToken ct = default) =>
        await _apiClient.PatchAsync<object>("api/orders/id", ct);
    public async Task<int> CreateOrderAsync(CreateOrderDto dto, CancellationToken ct = default) =>
        await _apiClient.PostAsync<int>("api/orders", ct);
}
