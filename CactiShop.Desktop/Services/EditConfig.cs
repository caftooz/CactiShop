namespace CactiShop.Desktop.Services;

internal class EditConfig
{
    public List<FieldConfig> FieldConfigs { get; set; } = [];
    public Action<object> OnChange { get; set; } = o => { };
    public Action<object> OnDelete { get; set; } = o => { };
}
