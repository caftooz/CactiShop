

using System.Windows;
using System.Windows.Controls;
using CactiShop.Contracts.Catalog;
using CactiShop.Contracts.OrderPoints;
using CactiShop.Contracts.Orders;
using CactiShop.Desktop.Enums;
using CactiShop.Desktop.Models;
using CactiShop.Desktop.Services;
using CactiShop.Desktop.Services.Interfaces;
using CactiShop.Desktop.Views;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CactiShop.Desktop.ViewModels;

internal partial class AdminViewModel
    (
        ICatalogService _catalogService,
        IOrderService _orderService,
        IAdminService _adminService
    ) : PageViewModel
{
    [ObservableProperty]
    private TabControl _adminTabControl;

    public override async Task OnNavigatedToAsync(object? parameter)
    {
        AdminTabControl = new TabControl();
        await InitializeAsync();
    }

    public async Task InitializeAsync()
    {
        var categoriesTask = _catalogService.GetCategoriesAsync();
        var orderStatusesTask = _adminService.GetOrderStatusesAsync();
        var orderPointsTask = _orderService.GetOrderPointsAsync();

        await Task.WhenAll(categoriesTask, orderStatusesTask, orderPointsTask);

        var categories = await categoriesTask;
        var orderStatuses = await orderStatusesTask;
        var orderPoints = await orderPointsTask;

        var productsDataTask = _catalogService.GetCatalogAsync(new ProductQuery());
        var ordersDataTask = _adminService.GetAllOrdersAsync();
        var reviewsDataTask = _adminService.GetAllReviewsAsync();

        await Task.WhenAll(productsDataTask, ordersDataTask, reviewsDataTask);

        var productsData = await productsDataTask;
        var ordersData = await ordersDataTask;
        var reviewsData = await reviewsDataTask;

        List<FieldConfig> productConfigs = new()
        {
            new FieldConfig("Id", nameof(ProductDetailsDto.Item.Id), EditType.ReadOnly),
            new FieldConfig("Название", nameof(ProductDetailsDto.Item.Name), EditType.Text),
            new FieldConfig("Категория", nameof(ProductDetailsDto.Item.CategoryName), EditType.Array, categories, nameof(CategoryDto.Name)),
            new FieldConfig("Цена", nameof(ProductDetailsDto.Item.Price), EditType.Number),
            new FieldConfig("Изображение", nameof(ProductDetailsDto.Item.ImageUrl), EditType.Image),
            new FieldConfig("Описание", nameof(ProductDetailsDto.Description), EditType.Text)
        };

        List<FieldConfig> categoryConfigs = new()
        {
            new FieldConfig("Id", nameof(Category.Id), EditType.ReadOnly),
            new FieldConfig("Название", nameof(Category.Name), EditType.Text)
        };

        List<FieldConfig> ordersConfigs = new()
        {
            new FieldConfig("Id", nameof(Order.Id), EditType.ReadOnly),
            new FieldConfig("Статус", nameof(Order.Status), EditType.Array, orderStatuses, nameof(OrderStatusDto.Name)),
            new FieldConfig("Пункт", nameof(Order.OrderPoint), EditType.Array, orderPoints, nameof(OrderPointDto.Address)),
            new FieldConfig("Доставка", nameof(Order.DeliveryAddress), EditType.Text)
        };

        List<FieldConfig> orderPointsConfigs = new()
        {
            new FieldConfig("Id", nameof(OrderPointDto.Id), EditType.ReadOnly),
            new FieldConfig("Адрес", nameof(OrderPointDto.Address), EditType.Text)
        };

        List<FieldConfig> productReviewsConfigs = new()
        {
            new FieldConfig("Id", nameof(ProductReviewListDto.Id), EditType.ReadOnly),
            new FieldConfig("Текст", nameof(ProductReviewListDto.Body), EditType.ReadOnly),
            new FieldConfig("Пользователь", nameof(ProductReviewListDto.Username), EditType.ReadOnly),
            new FieldConfig("Товар", nameof(ProductReviewListDto.ProductName), EditType.ReadOnly)
        };

        //Вот здесь вместо ProductSummary подгружается UpdateProductDto
        var productEditConfig = new EditConfig<object>(productConfigs,
            ps => _adminService.UpdateProductAsync(ToUpdateProductDto((ps as ProductDetailsDto)!)),
            ps => _adminService.DeleteProductAsync((ps as ProductDetailsDto)!.Item.Id));

        var categoryConfig = new EditConfig<Category>(categoryConfigs,
            c => _adminService.UpdateCategoryAsync(c),
            c => _adminService.DeleteCategoryAsync(c.Id));

        var orderConfig = new EditConfig<Order>(ordersConfigs,
            or => _adminService.UpdateOrderAsync(or),
            or => _adminService.DeleteOrderAsync(or.Id));

        var orderPointsConfig = new EditConfig<OrderPointDto>(orderPointsConfigs,
            op => _adminService.UpdateOrderPointAsync(op), op => _adminService.DeleteOrderPointAsync(op.Id));

        var productReviewsConfig = new EditConfig<ProductReviewListDto>(productReviewsConfigs,
            _ => MessageBox.Show("Отзыв можно только удалить"), pr => _adminService.DeleteReviewAsync(pr.Id));

        //Костылек
        Action<object> onProductEdit = async ps =>
        {
            await _catalogService.GetProductAsync((ps as ProductSummary)!.Id);
        };

        AdminTabControl.Items.Add(CreateTab("Товары", productsData, productEditConfig, onProductEdit));
        AdminTabControl.Items.Add(CreateTab("Категории", categories, categoryConfig));
        AdminTabControl.Items.Add(CreateTab("Заказы", ordersData, orderConfig));
        AdminTabControl.Items.Add(CreateTab("Пункты выдачи", orderPoints, orderPointsConfig));
        AdminTabControl.Items.Add(CreateTab("Отзывы", reviewsData, productReviewsConfig));
    }

    private TabItem CreateTab(string name, IEnumerable<object> collection, IEditConfig config, Action<object>? onEdit = null)
    {
        if (onEdit == null)
            onEdit = o => new UniversalEditWindow(o, config, _adminService).ShowDialog();

        var collectionGrid = new CollectionGridView(collection, onEdit);

        return new TabItem
        {
            Header = name,
            Content = collectionGrid
        };
    }

    private UpdateProductDto ToUpdateProductDto(ProductDetailsDto dto) =>
        new UpdateProductDto(
            dto.Item.Id,
            dto.Item.Name,
            dto.Item.CategoryName,
            dto.Item.Price,
            dto.Item.ImageUrl,
            dto.Description
        );
}
