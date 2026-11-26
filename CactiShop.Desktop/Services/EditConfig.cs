using CactiShop.Desktop.Services.Interfaces;

namespace CactiShop.Desktop.Services;

internal class EditConfig<T> : IEditConfig
{
    public List<FieldConfig> FieldConfigs { get; set; } = [];
    public Action<T> OnChange { get; set; } = o => { };
    public Action<T> OnDelete { get; set; } = o => { };

    public EditConfig(List<FieldConfig> fieldConfigs, Action<T> onChange, Action<T> onDelete)
    {
        FieldConfigs = fieldConfigs;
        OnChange = GetChecktypeAction(onChange);
        OnDelete = GetChecktypeAction(onDelete);
    }

    private Action<T> GetChecktypeAction(Action<T> act) => obj =>
    {
        if (obj is T) act(obj);
    };

    void IEditConfig.ExecuteChange(object obj)
    {
        if (obj is T typedObj) OnChange(typedObj);
    }

    void IEditConfig.ExecuteDelete(object obj)
    {
        if (obj is T typedObj) OnDelete(typedObj);
    }
}
