# GitHub API & Web Scraper Console App

Консольное приложение для работы с GitHub API и веб-скраппинга с сохранением результатов в SQLite базу данных.


## Структура проекта
```text
InternetTechLab1/
├── Database/                           # Работа с базой данных
│   └── AppDbContext.cs                  # Контекст БД (таблицы GitHubRepos, ScrapedPages)
│
├── Models/                             # Модели данных
│   ├── GitHubRepository.cs              # Модель репозитория (Id, Name, Stars, Url...)
│   ├── ScrapedPage.cs                   # Модель страницы (Title, H1, Description, Links...)
│   └── Setting.cs                       # Модель настроек (Database, CountPage, Timeout...)
│
├── Services/                           # Бизнес-логика
│   ├── GitHubService.cs                  # Работа с GitHub API (поиск репозиториев)
│   ├── WebScraper.cs                     # Веб-скраппинг (извлечение данных с сайтов)
│   └── SettingsService.cs                # Управление настройками (чтение/запись settings.json)
│
├── Program.cs                          # Точка входа, консольное меню, взаимодействие с пользователем
└── settings.json                       # Файл конфигурации (БД, количество результатов, таймаут)
```

## Возможности


### 🔹 GitHub API
- Поиск репозиториев по ключевым словам
- Просмотр информации о репозиториях (звёзды, язык, описание)
- Сохранение результатов в базу данных
- Открытие репозитория в браузере

### 🔹 Web Scraper
- Извлечение данных с веб-страниц:
  - Заголовок страницы (title)
  - Заголовок H1
  - Meta-описание
  - Ссылки на странице
  - Предпросмотр текста
- Сохранение результатов в базу данных

## Настройки

### Файл конфигурации `settings.json`

```json
{
  "database": "InternetTechLab1.db",
  "count_page": 10,
  "timeout": 30,
  "auto_save": false
}
```

| Параметр | Описание | Значение |
|----------|----------|----------|
| database | Имя файла БД | InternetTechLab1.db |
| count_page | Кол-во результатов | 10 |
| timeout | Таймаут (сек) | 30 |

## Технологии

- .NET 6/7/8
- Entity Framework Core (SQLite)
- GitHub API
- HtmlAgilityPack для парсинга HTML



