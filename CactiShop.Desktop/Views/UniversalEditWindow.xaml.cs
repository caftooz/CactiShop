using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CactiShop.Desktop.Enums;
using CactiShop.Desktop.Services;
using CactiShop.Desktop.Services.Interfaces;

namespace CactiShop.Desktop.Views;
/// <summary>
/// Универсальное окно для редактирования объекта
/// </summary>
public partial class UniversalEditWindow : Window
{
    private object _targetObject;
    internal UniversalEditWindow(object targetObject, IEditConfig config, IImageService imageService)
    {
        InitializeComponent();
        _targetObject = targetObject;

        DeleteButton.Click += (s, e) =>
        {
            config.ExecuteChange(s);
            Close();
        };
        SaveButton.Click += (s, e) =>
        {
            config.ExecuteDelete(s);
            Close();
        };
        foreach (FieldConfig fieldConfig in config.FieldConfigs)
        {
            TextBlock label = new TextBlock { Text = fieldConfig.Label };
            FieldsContainer.Children.Add(label);

            FrameworkElement inputControl;

            switch (fieldConfig.EditType)
            {
                case EditType.ReadOnly:
                    inputControl = new TextBlock();
                    inputControl.SetBinding(
                        TextBlock.TextProperty,
                        new Binding(fieldConfig.PropertyName)
                        {
                            Source = _targetObject
                        });
                    break;
                case EditType.Number:
                case EditType.Text:
                    inputControl = new TextBox();
                    inputControl.SetBinding(
                        TextBox.TextProperty,
                        new Binding(fieldConfig.PropertyName)
                        {
                            Source = _targetObject,
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                        });
                    break;
                case EditType.Date:
                    inputControl = new DatePicker();
                    inputControl.SetBinding(
                        DatePicker.SelectedDateProperty,
                        new Binding(fieldConfig.PropertyName)
                        {
                            Source = _targetObject,
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                        });
                    break;
                case EditType.CheckBox:
                    inputControl = new CheckBox { Content = "Да / Нет" };
                    inputControl.SetBinding(CheckBox.IsCheckedProperty, new Binding(fieldConfig.PropertyName) { Source = _targetObject });
                    break;
                case EditType.Array:
                    ComboBox comboBox = new ComboBox()
                    {
                        ItemsSource = fieldConfig.ItemsSource,
                        DisplayMemberPath = fieldConfig.MemberPath,
                        SelectedValuePath = fieldConfig.MemberPath
                    };

                    comboBox.SetBinding(
                        ComboBox.SelectedValueProperty,
                        new Binding(fieldConfig.PropertyName)
                        {
                            Source = _targetObject,
                            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                        });

                    inputControl = comboBox;
                    break;
                case EditType.Image:
                    StackPanel imageGroup = new StackPanel();

                    Image img = new Image { Height = 100, HorizontalAlignment = HorizontalAlignment.Left };
                    img.SetBinding(Image.SourceProperty, new Binding(fieldConfig.PropertyName)
                    {
                        Source = _targetObject,
                    });

                    imageGroup.Children.Add(img);

                    Button btnBrowse = new Button { Content = "Выбрать изображение...", Width = 150, HorizontalAlignment = HorizontalAlignment.Left };

                    btnBrowse.Click += async (s, args) =>
                    {
                        Microsoft.Win32.OpenFileDialog dlg = new();

                        dlg.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp;*.webp;";
                        if (dlg.ShowDialog() == true)
                        {
                            try
                            {
                                string serverFilename = await imageService.UploadImageAsync(dlg.FileName);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Ошибка при копировании картинки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    };
                    imageGroup.Children.Add(btnBrowse);

                    inputControl = imageGroup;
                    break;
            }
        }
    }
}
