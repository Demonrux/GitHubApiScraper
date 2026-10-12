using GitHubApiScraper.Services;
using GitHubApiScraper.UI;

namespace GitHubApiScraper.UI
{
    public class SettingsMenu
    {
        private readonly SettingsService _settingsService;

        public SettingsMenu(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public void Run()
        {
            while (true)
            {
                PrintMenu();

                switch (Console.ReadLine())
                {
                    case "1": EditCountPage(); break;
                    case "2": EditTimeout(); break;
                    case "3": EditGitHubDb(); break;
                    case "4": EditScraperDb(); break;
                    case "5": ResetToDefaults(); break;
                    case "6": return;
                }
            }
        }

        private void PrintMenu()
        {
            var s = _settingsService.CurrentSettings;
            ConsoleHelper.PrintHeader("Настройки");
            Console.WriteLine($"1. Количество результатов: {s.CountPage}");
            Console.WriteLine($"2. Таймаут (сек):          {s.Timeout}");
            Console.WriteLine($"3. Файл БД GitHub:         {s.GitHubReposDatabase}");
            Console.WriteLine($"4. Файл БД скраппера:      {s.ScrapingDatabase}");
            Console.WriteLine("5. Сбросить настройки");
            Console.WriteLine("6. Назад");
            Console.Write("\nВыберите: ");
        }

        private void EditCountPage()
        {
            Console.Write("Количество результатов (1-100): ");
            if (int.TryParse(Console.ReadLine(), out int count) && count is > 0 and <= 100)
            {
                _settingsService.UpdateSettings(s => s.CountPage = count);
                Console.WriteLine("Сохранено.");
            }
            else Console.WriteLine("Некорректное значение.");

            Thread.Sleep(1000);
        }

        private void EditTimeout()
        {
            Console.Write("Таймаут в секундах (5-120): ");
            if (int.TryParse(Console.ReadLine(), out int timeout) && timeout is >= 5 and <= 120)
            {
                _settingsService.UpdateSettings(s => s.Timeout = timeout);
                Console.WriteLine("Сохранено. Требуется перезапуск приложения.");
            }
            else Console.WriteLine("Некорректное значение.");

            Thread.Sleep(1000);
        }

        private void EditGitHubDb()
        {
            Console.Write("Имя файла БД GitHub: ");
            string? name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
            {
                _settingsService.UpdateSettings(s => s.GitHubReposDatabase = name);
                Console.WriteLine("Сохранено. Требуется перезапуск.");
            }
            Thread.Sleep(1000);
        }

        private void EditScraperDb()
        {
            Console.Write("Имя файла БД скрапера: ");
            string? name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
            {
                _settingsService.UpdateSettings(s => s.ScrapingDatabase = name);
                Console.WriteLine("Сохранено. Требуется перезапуск.");
            }
            Thread.Sleep(1000);
        }

        private void ResetToDefaults()
        {
            _settingsService.UpdateSettings(s =>
            {
                s.GitHubReposDatabase = "gitHubRepos.db";
                s.ScrapingDatabase = "scraping_docs.db";
                s.CountPage = 10;
                s.Timeout = 30;
            });
            Console.WriteLine("Настройки сброшены.");
            Thread.Sleep(1000);
        }
    }
}