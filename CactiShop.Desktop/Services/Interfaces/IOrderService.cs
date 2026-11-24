using CactiShop.Contracts.OrderPoints;
using CactiShop.Contracts.Orders;
using CactiShop.Desktop.Models;

namespace CactiShop.Desktop.Services.Interfaces;

internal interface IOrderService
{
    Task<IReadOnlyList<Order>> GetUserOrdersAsync(bool? byDescending = null, CancellationToken ct = default);
    Task<IReadOnlyList<OrderPointDto>> GetOrderPointsAsync(CancellationToken ct = default);
    Task<object> ChangeOrderStatusAsync(int orderdId, CancellationToken ct = default);
    Task<int> CreateOrderAsync(CreateOrderDto dto, CancellationToken ct = default);
}
