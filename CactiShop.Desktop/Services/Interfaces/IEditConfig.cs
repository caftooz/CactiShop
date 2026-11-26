namespace CactiShop.Desktop.Services.Interfaces;

internal interface IEditConfig
{
    List<FieldConfig> FieldConfigs { get; }
    void ExecuteChange(object obj);
    void ExecuteDelete(object obj);
}
