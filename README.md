## Порядок добавления новой страницы
1. **DTO** - добавить необходимые представления данных в `CactiShop.Contracts/`
2. **Модели** - для использования в WPF `Models/`.
3. **Интерфейс сервиса** - описание необходимых функций `Services/Interfaces/`
4. **Заглушка сервис** - для тестирования функционала `Services/Fake/`
5. **API сервис** - настоящий сервис для взаимодействия с API сервера - `Services/Api/`

(можно на этом этапе переключиться на CactiShop.Api и добавить контроллеры)

6. **Регистрация сервисов** - добавить в `App.xaml.cs` эти сервисы по строке на Fake и Api
7. **ViewModel** - в `ViewModels/` наследование от ==PageViewModel==. Зарегистрировать новую ==ViewModel== в DI контейнере (`App.xaml.cs`).
8. **View** - в `Views/`. Использовать UserControl.
9. **Связка** - присоединение ==ViewModel== к ==View== в `Resources/ViewTemplates.xaml`.

## Пояснения
> Пояснение для VM:
> Класс делаем `partial`, т.к. плагин MVVM автоматически будет генерировать вторую часть кода.
>
> Пример:
> ```cs
> public partial class ProductDetailsViewModel(
>    ICatalogService _catalogService, 
>    INavigationService _navigationService
>    ) : PageViewModel
> {...}
> ```
> Для обращения к сервисам использовать метод родительского класса RunAsync.
> Для того что бы сделать поле доступным во View необходимо указать ==[ObservableProperty]== перед этим полем, при этом публичное свойство будет генерироваться автоматически после сохранения.
> 
> Пример:
> ```cs
>[ObservableProperty]
>private ProductDetailsDto? _product;
>
>public override Task OnNavigatedToAsync(object? parameter) => RunAsync(async () =>
>{
>    if (parameter is not int id)
>        throw new ArgumentException("Карточке товара нужен id товара.");
>        
>    Product = await _catalogService.GetProductAsync(id);
>});
> ```
> 
> Что бы использовать команды необходимо перед методом указать ==[RelayCommand]==.
>
>Пример:
>``` cs
>[RelayCommand]
>private void GoBack() => _navigationService.GoBack();
>```
