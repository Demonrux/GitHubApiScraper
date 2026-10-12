using GitHubApiScraper.Database;
using GitHubApiScraper.Models;
using GitHubApiScraper.Services;
using System.Text.Json;

namespace GitHubApiScraper.UI
{
    public class ScraperMenu
    {
        private readonly WebScraper _scraper;
        private readonly ScrapingDb _db;

        public ScraperMenu(WebScraper scraper, ScrapingDb db)
        {
            _scraper = scraper;
            _db = db;
        }

        public async Task RunAsync()
        {
            while (true)
            {
                ConsoleHelper.PrintHeader("Web Scraping");
                Console.WriteLine("1. Скрапить URL");
                Console.WriteLine("2. Показать сохраненные");
                Console.WriteLine("3. Назад");
                Console.Write("Выберите пункт: ");

                switch (Console.ReadLine())
                {
                    case "1": await ScrapeAsync(); break;
                    case "2": ShowSaved(); break;
                    case "3": return;
                }
            }
        }

        private async Task ScrapeAsync()
        {
            ConsoleHelper.PrintHeader("Скрапинг URL");
            Console.Write("Введите URL: ");
            string? url = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(url))
            {
                ConsoleHelper.Pause("URL не может быть пустым.");
                return;
            }

            var result = await _scraper.ScrapeUrlAsync(url);

            if (result == null)
            {
                ConsoleHelper.Pause("Не удалось загрузить страницу.");
                return;
            }

            PrintResult(result);

            if (ConsoleHelper.AskYesNo("Сохранить в БД?"))
            {
                _db.SavePage(result);
                Console.WriteLine("Сохранено в документную БД.");
            }

            ConsoleHelper.Pause();
        }

        private static void PrintResult(ScrapedPage result)
        {
            Console.WriteLine("\n=== Результат ===");
            Console.WriteLine($"Заголовок: {result.Title}");
            Console.WriteLine($"H1:        {result.H1 ?? "Не найден"}");
            Console.WriteLine($"Описание:  {result.Description ?? "Не найдено"}");

            var links = JsonSerializer.Deserialize<List<string>>(result.LinksJson ?? "[]");
            Console.WriteLine($"Ссылок:    {links?.Count ?? 0}");

            if (!string.IsNullOrEmpty(result.ContentPreview))
                Console.WriteLine($"\nТекст:\n{result.ContentPreview}");
        }

        private void ShowSaved()
        {
            ConsoleHelper.PrintHeader("Сохраненные страницы");

            var pages = _db.GetAllPages();

            if (pages.Count == 0)
            {
                ConsoleHelper.Pause("Нет сохраненных страниц.");
                return;
            }

            Console.WriteLine($"Всего в БД: {_db.GetTotalCount()}\n");

            for (int i = 0; i < pages.Count; i++)
            {
                var page = pages[i];
                Console.WriteLine($"{i + 1}. {page.Title}");
                Console.WriteLine($"   {page.ScrapedAt:dd.MM.yyyy HH:mm}");
                Console.WriteLine($"   {page.Url}\n");
            }

            Console.WriteLine("0. Назад");
            Console.Write("Введите номер для детального просмотра: ");

            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= pages.Count)
                ShowDetails(pages[index - 1]);
        }

        private void ShowDetails(ScrapedPage page)
        {
            ConsoleHelper.PrintHeader("Детали страницы");
            Console.WriteLine($"Заголовок: {page.Title}");
            Console.WriteLine($"URL:       {page.Url}");
            Console.WriteLine($"Дата:      {page.ScrapedAt:dd.MM.yyyy HH:mm}");

            if (!string.IsNullOrEmpty(page.H1))
                Console.WriteLine($"H1:        {page.H1}");

            if (!string.IsNullOrEmpty(page.Description))
                Console.WriteLine($"Описание:  {page.Description}");

            var links = JsonSerializer.Deserialize<List<string>>(page.LinksJson ?? "[]");
            if (links != null && links.Any())
            {
                Console.WriteLine($"\nСсылки ({links.Count}):");
                foreach (var link in links.Take(40))
                    Console.WriteLine($"  - {link}");
            }

            if (!string.IsNullOrEmpty(page.ContentPreview))
                Console.WriteLine($"\nТекст:\n{page.ContentPreview}");

            ConsoleHelper.Pause();
        }
    }
}