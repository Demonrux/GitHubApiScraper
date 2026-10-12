using GitHubApiScraper.Database;
using GitHubApiScraper.Services;
using GitHubApiScraper.UI;

class Program
{
    static async Task Main()
    {
        SettingsService settingsService = new SettingsService();

        using GitHubReposDb apiDb = new GitHubReposDb(settingsService.CurrentSettings.GitHubReposDatabase);
        apiDb.Database.EnsureCreated();

        ScrapingDb documentDb = new ScrapingDb(settingsService.CurrentSettings.ScrapingDatabase);

        using HttpClient httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "GitHubApiScraper-App");
        httpClient.Timeout = TimeSpan.FromSeconds(settingsService.CurrentSettings.Timeout);

        GitHubService githubService = new GitHubService(httpClient);
        WebScraper webScraper = new WebScraper(httpClient);

        ConsoleUI app = new ConsoleUI(githubService, webScraper, apiDb, documentDb, settingsService);
        await app.RunAsync();
    }
}