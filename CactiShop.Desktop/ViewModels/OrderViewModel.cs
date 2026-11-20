using CactiShop.Contracts.Orders;
using CactiShop.Desktop.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CactiShop.Desktop.ViewModels;

internal partial class OrderViewModel(
    IOrderService _orderService,
    INavigationService _navigationService
    ) : PageViewModel
{
    public ObservableCollection<OrderDto> Orders { get; } = new();
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    private bool _isLoaded = false;
    public bool IsEmpty => IsLoaded && Orders.Count == 0;
    public override Task OnNavigatedToAsync(object? parameter) => RunAsync(async () =>
    {
        var orders = await _orderService.GetUserOrdersAsync();
        Orders.Clear();
        foreach(var order in orders) Orders.Add(order);
        IsLoaded = true;
    });
}
