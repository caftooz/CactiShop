namespace CactiShop.Desktop.Services.Interfaces;

public interface IDialogService
{
    void ShowInfo(string message);
    void ShowError(string message);
    bool Confirm(string question);

    /// <summary>
    ///     Выбрать файл изображения и вернуть путь к нему. Если пользователь отменяет выбор, возвращается null.
    /// </summary>
    string? PickImageFile();
}
