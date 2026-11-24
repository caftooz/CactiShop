

using System.Windows.Controls;
using CactiShop.Contracts.Catalog;
using CactiShop.Contracts.OrderPoints;
using CactiShop.Contracts.Orders;
using CactiShop.Desktop.Enums;
using CactiShop.Desktop.Models;
using CactiShop.Desktop.Services;
using CactiShop.Desktop.Services.Interfaces;
using CactiShop.Desktop.Views;

namespace CactiShop.Desktop.ViewModels;



internal class AdminViewModel
{
    private TabControl _tabControl;
    private IImageService _imageService;
    private Dictionary<string, Func<Task<IEnumerable<object>>>> _tabLists = new();

    internal AdminViewModel(IImageService imageService, ICatalogService catalogService, IOrderService orderService, IAdminService adminService)
    {
        _tabControl = new TabControl();
        _imageService = imageService;

        _tabLists.Add("Товары", async () => await catalogService.GetCatalogAsync(new ProductQuery()));
        _tabLists.Add("Категории", async () => await catalogService.GetCategoriesAsync());
        _tabLists.Add("Заказы", async () => await adminService.GetAllOrdersAsync());
        _tabLists.Add("Пункты выдачи", async () => await orderService.GetOrderPointsAsync());
        _tabLists.Add("Отзывы", async () => await adminService.GetAllReviewsAsync());

        var categories = catalogService.GetCategoriesAsync().GetAwaiter().GetResult();

        List<FieldConfig> productConfigs = new()
        {
            new FieldConfig("Id", nameof(ProductSummary.Id), EditType.ReadOnly),
            new FieldConfig("Название", nameof(ProductSummary.Name), EditType.Text),
            new FieldConfig("Категория", nameof(ProductSummary.CategoryName), EditType.Array, categories, nameof(CategoryDto.Name)),
            new FieldConfig("Цена", nameof(ProductSummary.Price), EditType.Number),
            new FieldConfig("Изображение", nameof(ProductSummary.ImageUrl), EditType.Image)
        };

        List<FieldConfig> categoryConfigs = new()
        {
            new FieldConfig("Id", nameof(Category.Id), EditType.ReadOnly),
            new FieldConfig("Название", nameof(Category.Name), EditType.Text)
        };

        var orderStatuses = adminService.GetOrderStatusesAsync().GetAwaiter().GetResult();
        var orderPoints = orderService.GetOrderPointsAsync().GetAwaiter().GetResult();

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

        var productEditConfig = new EditConfig
        {
            FieldConfigs = productConfigs,
            OnChange = o =>
            {
                if (o is ProductSummary ps)
                {
                    adminService.UpdateProduct(ps);
                }
            },
            OnDelete = o =>
            {
                if (o is ProductSummary ps)
                {
                    adminService.DeleteProduct(ps.Id);
                }
            }
        };
    }

    private TabItem ChangeTab(string name, IEnumerable<object> collection, EditConfig config)
    {
        TabItem tabItem = new TabItem();

        Action<object> onEdit = o => new UniversalEditWindow(o, config, _imageService).ShowDialog();

        var grid = new CollectionGridView(collection, onEdit);

        tabItem.Content = grid;

        return tabItem;
    }
}
