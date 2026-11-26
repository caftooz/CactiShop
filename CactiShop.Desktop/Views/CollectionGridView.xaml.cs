using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CactiShop.Desktop.Views;
/// <summary>
/// UserControl с таблицей из элементов коллекции <paramref name="collection"/>,
/// <br/>Автоматически помещает колонку "изменить" с кнопкой, при нажатии на которую вызывается <paramref name="onEditButton"/>
/// </summary>
public partial class CollectionGridView : UserControl
{
    private IEnumerable<object> _collection;
    private Action<object> _onEditButton;
    public CollectionGridView(IEnumerable<object> collection, Action<object> onEditButton)
    {
        InitializeComponent();
        _collection = collection;
        _onEditButton = onEditButton;

        BuildDataGrid();
    }

    private void BuildDataGrid()
    {
        var dataGrid = CollectionDataGrid;

        dataGrid.ItemsSource = _collection;
        var columns = dataGrid.Columns;

        var properties = typeof(object).GetProperties();

        foreach (var property in properties)
        {
            var column = new DataGridTextColumn { Header = property.Name, Binding = new Binding(property.Name) };
            columns.Add(column);
        }

        var buttonColumn = new DataGridTemplateColumn { Header = "Действие" };
        FrameworkElementFactory buttonFactory = new FrameworkElementFactory(typeof(Button));
        buttonFactory.SetValue(Button.ContentProperty, "Изменить");
        buttonFactory.SetValue(Button.MarginProperty, new Thickness(2));

        buttonFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler((sender, e) =>
        {
            Button clickedButton = (Button)sender;

            var selectedItem = clickedButton.DataContext;

            _onEditButton?.Invoke(selectedItem);
        }));

        DataTemplate cellTemplate = new DataTemplate { VisualTree = buttonFactory };
        buttonColumn.CellTemplate = cellTemplate;

        dataGrid.Columns.Add(buttonColumn);
    }
}
