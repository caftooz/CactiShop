# Как добавить новую страницу в Desktop

Порядок шагов для новой страницы клиента (например, «Корзина»). Общее устройство клиента описано в [README](../README.md#desktop).

## Порядок

1. **DTO** — описать данные, которые приходят от API, в `CactiShop.Contracts/`.
2. **Маршрут** — добавить адрес эндпоинта в `CactiShop.Contracts/ApiRoutes.cs`.
3. **Модели** — данные в том виде, в каком они нужны интерфейсу, в `CactiShop.Desktop/Models/`.
4. **Интерфейс сервиса** — какие операции нужны странице, в `Services/Interfaces/`.
5. **Fake-сервис** — реализация на тестовых данных в `Services/Fake/`. С ней можно сделать и проверить всю страницу, пока API ещё не готово.
6. **API-сервис** — настоящая реализация в `Services/Api/`: вызывает `ApiClient` и переводит DTO в модели. Адреса — только из `ApiRoutes`, без строк вручную.

   *(На этом шаге можно переключиться на `CactiShop.Api` и добавить контроллер.)*

7. **Регистрация сервисов** — в `App.xaml.cs` по строке для Fake- и для API-реализации.
8. **ViewModel** — в `ViewModels/`, наследуется от `PageViewModel`. Зарегистрировать её в DI-контейнере в `App.xaml.cs`.
9. **View** — в `Views/`, это `UserControl`.
10. **Связка** — добавить `DataTemplate` «ViewModel → View» в `Resources/ViewTemplates.xaml`.

## Пояснения

### ViewModel

Класс делаем `partial`: генератор CommunityToolkit.Mvvm создаёт вторую часть кода автоматически.

```cs
public partial class ProductDetailsViewModel(
    ICatalogService _catalogService,
    INavigationService _navigationService
    ) : PageViewModel
{
    ...
}
```

### Обращение к сервисам

Только через метод родительского класса `RunAsync`. Он показывает индикатор загрузки и превращает ошибки в текст для `ErrorMessage`.

### Свойства для View

Чтобы поле было доступно во View, перед ним ставится `[ObservableProperty]`. Публичное свойство генерируется автоматически после сохранения файла: из `_product` получится `Product`.

```cs
[ObservableProperty]
private ProductDetails? _product;

public override Task OnNavigatedToAsync(object? parameter) => RunAsync(async () =>
{
    if (parameter is not int id)
        throw new ArgumentException("Карточке товара нужен id товара.");

    Product = await _catalogService.GetProductAsync(id);
});
```

### Команды

Чтобы метод можно было вызвать из View, перед ним ставится `[RelayCommand]`. Генерируется свойство `GoBackCommand`, к нему привязывается кнопка: `Command="{Binding GoBackCommand}"`.

```cs
[RelayCommand]
private void GoBack() => _navigationService.GoBack();
```

Асинхронный метод (`Task`) тоже подходит. Пока команда выполняется, кнопка автоматически становится неактивной.

### Переход на страницу

```cs
await _navigationService.NavigateToAsync<ProductDetailsViewModel>(productId);
```

Параметр придёт в `OnNavigatedToAsync(object? parameter)` новой страницы.
