using CactiShop.Desktop.Enums;

namespace CactiShop.Desktop.Services;

public class FieldConfig
{
    public string Label { get; set; }
    public string PropertyName { get; set; }
    public EditType EditType { get; set; }
    public string MemberPath { get; set; }

    public IEnumerable<object>? ItemsSource { get; set; }

    public FieldConfig(string label, string propertyName, EditType editType, IEnumerable<object>? itemsSource = null, string memberPath = "Name")
    {
        Label = label;
        PropertyName = propertyName;
        EditType = editType;
        MemberPath = memberPath;
        ItemsSource = itemsSource;
    }
}
