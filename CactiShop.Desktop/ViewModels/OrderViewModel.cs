using System.Collections.ObjectModel;
using CactiShop.Desktop.Models;
using CactiShop.Desktop.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CactiShop.Desktop.ViewModels;

internal partial class OrderViewModel(
    IOrderService _orderService
    ) : PageViewModel
{
    public ObservableCollection<Order> Orders { get; } = new();
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    private bool _isLoaded = false;
    public bool IsEmpty => IsLoaded && Orders.Count == 0;
    public override Task OnNavigatedToAsync(object? parameter) => RunAsync(async () =>
    {
        var orders = await _orderService.GetUserOrdersAsync();
        Orders.Clear();
        foreach (var order in orders) Orders.Add(order);
        IsLoaded = true;
    });
}
