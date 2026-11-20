using CactiShop.Contracts.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CactiShop.Desktop.Services.Interfaces;

internal interface IOrderService
{
    Task<IReadOnlyList<OrderDto>> GetUserOrdersAsync(bool? byDescending = null, CancellationToken ct = default);
    Task<IReadOnlyList<OrderDto>> GetAllOrdersAsync(CancellationToken ct = default);
    Task<object> ChangeOrderStatusAsync(int orderdId, CancellationToken ct = default);
    Task<int> CreateOrderAsync(CreateOrderDto dto, CancellationToken ct = default);
}