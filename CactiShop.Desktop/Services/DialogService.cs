using CactiShop.Desktop.Services.Interfaces;
using System.Windows;

namespace CactiShop.Desktop.Services;

public class DialogService : IDialogService
{
    public void ShowInfo(string message)
        => MessageBox.Show(message, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
    public void ShowError(string message)
        => MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    public bool Confirm(string question)
        => MessageBox.Show(question, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
    public string? PickImageFile()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = "Выберите фото",
            Filter = "Изображения (*.jpg;*.jpeg;*.png;*.webp)|*.jpg;*.jpeg;*.png;*.webp"
        };
        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
}
