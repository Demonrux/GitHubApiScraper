using GitHubApiScraper.Database;
using GitHubApiScraper.Services;

namespace GitHubApiScraper.UI
{
    public class ConsoleUI
    {
        private readonly GitHubService _githubService;
        private readonly WebScraper _webScraper;
        private readonly GitHubReposDb _apiDb;
        private readonly ScrapingDb _documentDb;
        private readonly SettingsService _settingsService;

        public ConsoleUI(
            GitHubService githubService,
            WebScraper webScraper,
            GitHubReposDb apiDb,
            ScrapingDb documentDb,
            SettingsService settingsService)
        {
            _githubService = githubService;
            _webScraper = webScraper;
            _apiDb = apiDb;
            _documentDb = documentDb;
            _settingsService = settingsService;
        }

        public async Task RunAsync()
        {
            while (true)
            {
                ConsoleHelper.PrintHeader("Главное меню");
                Console.WriteLine("1. GitHub API");
                Console.WriteLine("2. Web Scraping");
                Console.WriteLine("3. Настройки");
                Console.WriteLine("4. Выход");
                Console.Write("Выберите пункт: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        await new GitHubMenu(_githubService, _apiDb, _settingsService).RunAsync();
                        break;
                    case "2":
                        await new ScraperMenu(_webScraper, _documentDb).RunAsync();
                        break;
                    case "3":
                        new SettingsMenu(_settingsService).Run();
                        break;
                    case "4":
                        return;
                }
            }
        }
    }
}