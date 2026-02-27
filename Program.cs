using InternetTechLab1.Database;
using InternetTechLab1.Models;
using InternetTechLab1.Services;
using System.Text.Json;

SettingsService settingsService = new SettingsService();

var db = new AppDbContext(settingsService.CurrentSettings.Database);
db.Database.EnsureCreated();

HttpClient httpClient = new HttpClient();  
httpClient.DefaultRequestHeaders.Add("User-Agent", "InternetTechLab1-App");
httpClient.Timeout = TimeSpan.FromSeconds(settingsService.CurrentSettings.Timeout);

GitHubService githubService = new GitHubService(httpClient);  
WebScraper webScraper = new WebScraper(httpClient);       


while (true)
{
    Console.Clear();
    Console.WriteLine("1.GitHub API");
    Console.WriteLine("2.Web Scraping");
    Console.WriteLine("3.Настройки");
    Console.WriteLine("4.Выход");
    Console.Write("Выберите: ");

    var choice = Console.ReadLine();

    if (choice == "1")
    {
        await GitHubMenu(githubService, db);
    }
    else if (choice == "2")
    {
        await ScrapingMenu(webScraper, db);
    }
    else if (choice == "3")
    {
        ShowSettings(settingsService);
    }
    else if (choice == "4")
    {
        break;
    }
}

async Task GitHubMenu(GitHubService service, AppDbContext db)
{
    while (true)
    {
        Console.Clear();

        Console.WriteLine("1.Поиск репозиториев");
        Console.WriteLine("2.Показать сохраненные");
        Console.WriteLine("3.Назад");
        Console.Write("Выберите: ");

        var choice = Console.ReadLine();

        if (choice == "1")
        {
            await SearchRepositories(service, db);
        }
        else if (choice == "2")
        {
            ShowSavedRepositories(db);
        }
        else if (choice == "3")
        {
            break;
        }
    }
}

async Task ScrapingMenu(WebScraper scraper, AppDbContext db)
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine("1.Скрапить URL");
        Console.WriteLine("2.Показать сохраненные");
        Console.WriteLine("3.Назад");
        Console.Write("Выберите: ");

        var choice = Console.ReadLine();

        if (choice == "1")
        {
            await ScrapeUrl(scraper, db);
        }
        else if (choice == "2")
        {
            ShowScrapedPages(db);
        }
        else if (choice == "3")
        {
            break;
        }
    }
}

async Task SearchRepositories(GitHubService service, AppDbContext db)
{
    Console.Clear();
    Console.WriteLine("=== Поиск ===");

    Console.Write("Запрос: ");
    string? query = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(query)) return;

    Console.WriteLine("Поиск...");

    try
    {
        List<GitHubRepository> repositories = await service.SearchRepositoriesAsync(query, settingsService.CurrentSettings.CountPage);

        if (repositories.Count == 0)
        {
            Console.WriteLine("Не найдено");
            Console.ReadKey();
            return;
        }

        Console.Write("\nСохранить? (y/n): ");
        if (Console.ReadLine()?.ToLower() == "y")
        {
            int saved = 0;
            foreach (GitHubRepository repository in repositories)
            {
                if (!db.GitHubRepos.Any(repo => repo.Id == repository.Id))
                {
                    db.GitHubRepos.Add(repository);
                    saved++;
                }
            }
            db.SaveChanges();
            Console.WriteLine($"Сохранено: {saved}");
        }

        Console.ReadKey();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\nJшибка: {ex.Message}");
        Console.WriteLine("Нажмите любую клавишу...");
        Console.ReadKey();
    }
}

void ShowSavedRepositories(AppDbContext db)
{
    while (true) 
    {
        Console.Clear();
        Console.WriteLine("Сохраненные репозитории:");

        int totalCount = db.GitHubRepos.Count();
        Console.WriteLine($"Всего: {totalCount}");

        IQueryable<GitHubRepository> query = db.GitHubRepos;

        List<GitHubRepository> repositories = query.OrderByDescending(r => r.Stars).Take(50).ToList();
        Console.WriteLine("Введите номер для просмотра (0 - назад):");

        for (int i = 0; i < repositories.Count; i++)
        {
            GitHubRepository repo = repositories[i];
            Console.WriteLine($"{i + 1}. {repo.Name} ({repo.Stars} stars) [{repo.Language ?? ""}]");
        }

        Console.Write("\nВыберите номер (или 0 для возврата): ");

        if (int.TryParse(Console.ReadLine(), out int selectedIndex) && selectedIndex > 0 && selectedIndex <= repositories.Count)
        {
            ShowRepositoryDetails(repositories[selectedIndex - 1]);
        }
        else if (selectedIndex == 0)
        {
            break; 
        }
    }
}

void ShowRepositoryDetails(GitHubRepository repository)
{
    Console.Clear();
    Console.WriteLine("Детальная информация:");
    Console.WriteLine($"\nНазвание: {repository.Name}");
    Console.WriteLine($"Описание: {repository.Description}");
    Console.WriteLine($"Звезды: {repository.Stars}");
    Console.WriteLine($"Язык: {repository.Language ?? "Не указан"}");
    Console.WriteLine($"Владелец: {repository.OwnerLogin}");
    Console.WriteLine($"Создан: {repository.CreatedAt:dd.MM.yyyy}");
    Console.WriteLine($"URL: {repository.Url}");
    Console.WriteLine($"ID: {repository.Id}");

    Console.WriteLine("\nДействия:");
    Console.WriteLine("1.Открыть в браузере");
    Console.WriteLine("2.Назад к списку");
    Console.Write("Выберите: ");

    string? choice = Console.ReadLine();

    if (choice == "1")
    {
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = repository.Url,
                UseShellExecute = true
            });
        }
        catch
        {
            Console.WriteLine("Не удалось открыть браузер");
        }
        Console.ReadKey();
    }
}

async Task ScrapeUrl(WebScraper scraper, AppDbContext db)
{
    Console.Clear();

    Console.Write("Введите URL: ");
    string? url = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(url))
    {
        Console.WriteLine("URL не может быть пустым");
        Console.ReadKey();
        return;
    }

    ScrapedPage? result = await scraper.ScrapeUrlAsync(url);

    if (result == null)
    {
        Console.WriteLine("\nНе удалось загрузить страницу");
        Console.ReadKey();
        return;
    }

    Console.WriteLine("\nРезультат скрапинга:");
    Console.WriteLine($"Заголовок: {result.Title}");
    Console.WriteLine($"H1: {result.H1 ?? "Не найден"}");
    Console.WriteLine($"Описание: {result.Description ?? "Не найдено"}");

    List<string>? links = JsonSerializer.Deserialize<List<string>>(result.LinksJson ?? "[]");
    Console.WriteLine($"Всего ссылок: {links?.Count ?? 0}");

    if (!string.IsNullOrEmpty(result.ContentPreview))
    {
        Console.WriteLine($"\nТекст:\n{result.ContentPreview}");
    }

    Console.Write("\nСохранить в БД? (y/n): ");
    if (Console.ReadLine()?.ToLower() == "y")
    {
        db.ScrapedPages.Add(result);
        await db.SaveChangesAsync();
    }

    Console.ReadKey();
}

void ShowScrapedPages(AppDbContext db)
{
    Console.Clear();
    Console.WriteLine("Сохраненные страницы:\n");

    List<ScrapedPage> pages = db.ScrapedPages.OrderByDescending(p => p.ScrapedAt).ToList();

    if (!pages.Any())
    {
        Console.WriteLine("Нет сохраненных страниц");
        Console.ReadKey();
        return;
    }

    Console.WriteLine($"Всего в БД: {db.ScrapedPages.Count()}\n");

    for (int i = 0; i < pages.Count; i++)
    {
        ScrapedPage page = pages[i];
        Console.WriteLine($"{i + 1}. {page.Title}");
        Console.WriteLine($"{page.ScrapedAt:dd.MM.yyyy HH:mm}");
        Console.WriteLine($"{page.Url}");
        Console.WriteLine();
    }

    Console.WriteLine("0.Назад");
    Console.Write("Введите номер для детального просмотра: ");

    if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= pages.Count)
    {
        ShowScrapedPageDetails(pages[index - 1]);
    }
}

void ShowScrapedPageDetails(ScrapedPage page)
{
    Console.Clear();

    Console.WriteLine($"Заголовок: {page.Title}");
    Console.WriteLine($"URL: {page.Url}");
    Console.WriteLine($"Дата: {page.ScrapedAt:dd.MM.yyyy HH:mm}");

    if (!string.IsNullOrEmpty(page.H1))
        Console.WriteLine($"H1: {page.H1}");

    if (!string.IsNullOrEmpty(page.Description))
        Console.WriteLine($"Description: {page.Description}");

    List<string>? links = JsonSerializer.Deserialize<List<string>>(page.LinksJson ?? "[]");
    if (links != null && links.Any())
    {
        Console.WriteLine($"\nСсылки ({links.Count}):");
        foreach (string link in links.Take(40))
        {
            Console.WriteLine($".{link}");
        }
    }

    if (!string.IsNullOrEmpty(page.ContentPreview))
    {
        Console.WriteLine($"\nТекст:\n{page.ContentPreview}");
    }
    Console.ReadKey();
}

void ShowSettings(SettingsService settingsService)
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine("Настройки:");
        Console.WriteLine($"1.Количество результатов: {settingsService.CurrentSettings.CountPage}");
        Console.WriteLine($"2.Таймаут: {settingsService.CurrentSettings.Timeout}");
        Console.WriteLine($"3.Файл БД: {settingsService.CurrentSettings.Database}");
        Console.WriteLine("4.Сбросить настройки");
        Console.WriteLine("5.Назад");
        Console.Write("Выберите: ");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                Console.Write("Количество результатов (1-100): ");
                if (int.TryParse(Console.ReadLine(), out int count) && count > 0 && count <= 100)
                {
                    settingsService.UpdateSettings(s => s.CountPage = count);
                }
                break;

            case "2":
                Console.Write("Таймаут в секундах (5-120): ");
                if (int.TryParse(Console.ReadLine(), out int timeout) && timeout >= 5 && timeout <= 120)
                {
                    settingsService.UpdateSettings(s => s.Timeout = timeout);
                }
                break;

            case "3":
                Console.Write("Имя файла БД (например: mydb.db):");
                string? dbName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(dbName))
                {
                    settingsService.UpdateSettings(s => s.Database = dbName);
                }
                break;

            case "4":
                settingsService.UpdateSettings(s =>
                {
                    s.Database = "InternetTechLab1.db";
                    s.CountPage = 10;
                    s.Timeout = 30;
                });
                Console.WriteLine("Настройки сброшены");
                Thread.Sleep(1000);
                break;

            case "5":
                return;
        }
    }
}