Короткие правила для всей команды. Прочитайте целиком перед задачей.

## Коротко

1. Задача делается в отдельной ветке `feature/...`, созданной от свежего `dev`.
2. Изменения попадают в `dev` **только через Pull Request**.
3. PR сливается, когда CI зелёный и PR одобрил `@caftooz`.
4. После слияния ветка больше не используется: следующая задача — новая ветка.

### Как называть ветки

`feature/<что-делаем>` — латиница, нижний регистр, слова через дефис.

```
feature/cart-page          ✅
feature/api-orders         ✅
Feature/CartPage           ❌  регистр
fix-cart                   ❌  не feature/
```

## Работа над задачей в Visual Studio

### 1. Начать задачу

1. Переключитесь на `dev`.
2. **Git Changes -> Pull** — получить свежий `dev`.
3. **Git -> New Branch…**
   - Branch name: `feature/<что-делаем>`
   - Based on: `dev`

### 2. Работать

- Коммитьте небольшими логическими шагами.

### 3. Открыть Pull Request

1. **Push** → на плашке **Create in Browser**.
2. Проверьте направление: `feature/cart-page` → **`dev`**.
3. **Заголовок PR** пишите понятно: при слиянии он станет сообщением коммита в `dev`.

   Хорошо: `Add cart page`, `Fix price filter in catalog`. Плохо: `fix`, `HotFix`, `123`.
4. Заполните шаблон: что сделано, отметьте пункты проверки.

### 4. Пока PR на проверке

- Дождитесь зелёного **CI** (сборка + проверка форматирования).
- Если пришёл коммит **`style: dotnet format`** от бота, **сделайте Pull** в своей ветке, прежде чем продолжать. Иначе следующий Push будет отклонён.
- На замечания ревью отвечайте новыми коммитами в ту же ветку и делайте Push. PR обновится сам, новый открывать не нужно.

### 5. После слияния

PR сливается **одним коммитом (squash)**, ветка на GitHub удаляется автоматически.

1. Переключитесь на `dev` → **Pull**.
2. Удалите свою локальную ветку: окно **Git Repository** → правый клик по ветке → **Delete**.
   VS предупредит, что ветка «не полностью слита». Это нормально при squash, подтверждайте.
3. **Не продолжайте работу в старой ветке.** Следующая задача — снова с пункта 1.

## Если `dev` ушёл вперёд, пока вы работали

Самый простой способ — кнопка **Update branch** на странице PR.

Или в Visual Studio:
1. **Git Repository** → **Fetch**.
2. Находясь в своей ветке: правый клик по `origin/dev` → **Merge 'origin/dev' into 'feature/...'**.
3. Если есть конфликты, VS откроет редактор слияния. Разберите каждый файл → **Accept Merge** → коммит → Push.

## Стиль кода

- Включите один раз: *Tools -> Options -> Text Editor -> Code Cleanup* -> ☑ **Run Code Cleanup profile on Save**.
  В профиле (*Configure Code Cleanup*): **Format document**, **Sort usings**, **Remove unnecessary usings**.

## Первый запуск проекта

### Клиент без сервера (Fake-режим)

В `CactiShop.Desktop/appsettings.json` поставьте `"UseFake": true`. Клиент будет работать на тестовых данных. Сервер и база не нужны.

### API и база

Нужен PostgreSQL на вашем компьютере.

**Секреты не хранятся в репозитории.** Один раз в папке `CactiShop.Api`:

1. Сгенерируйте ключ (PowerShell):
   ```powershell
   $bytes = New-Object byte[] 64
   [System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
   [Convert]::ToBase64String($bytes)
   ```
2. Сохраните секреты. `Password` — пароль вашего **локального** PostgreSQL:

В Visual Studio: правый клик по `CactiShop.Api` -> Manage User Secrets.
``` json
{
  "Jwt:Key": "<ключ>",
  "ConnectionStrings:DefaultConnection": "Host=localhost;Port=5432;Database=cactishop;Username=postgres;Password=<ваш пароль>"
}
```
3. Создайте базу:
   ```powershell
   $env:ASPNETCORE_ENVIRONMENT = "Development"
   dotnet ef database update
   ```

## Нельзя

- ❌ Коммитить в `dev` или `master`. Даже если «одна строчка».
- ❌ Продолжать работать в ветке, которую уже слили.
- ❌ Создавать новую ветку от чужой несслитой ветки (только от `dev`).
- ❌ Класть в репозиторий пароли, ключи, строки подключения с паролем.
- ❌ Делать PR, в котором «всё сразу». Одна задача — один PR. Маленькие PR быстрее проверяются.
