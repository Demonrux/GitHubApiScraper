using GitHubApiScraper.Database;
using GitHubApiScraper.Models;
using GitHubApiScraper.Services;


namespace GitHubApiScraper.UI
{
    public class GitHubMenu
    {
        private readonly GitHubService _service;
        private readonly GitHubReposDb _db;
        private readonly SettingsService _settingsService;

        public GitHubMenu(GitHubService service, GitHubReposDb db, SettingsService settingsService)
        {
            _service = service;
            _db = db;
            _settingsService = settingsService;
        }

        public async Task RunAsync()
        {
            while (true)
            {
                ConsoleHelper.PrintHeader("GitHub API");
                Console.WriteLine("1. Поиск репозиториев");
                Console.WriteLine("2. Показать сохраненные");
                Console.WriteLine("3. Назад");
                Console.Write("Выберите пункт: ");

                switch (Console.ReadLine())
                {
                    case "1": await SearchAsync(); break;
                    case "2": ShowSaved(); break;
                    case "3": return;
                }
            }
        }

        private async Task SearchAsync()
        {
            ConsoleHelper.PrintHeader("Поиск репозиториев");
            Console.Write("Введите поисковый запрос: ");
            string? query = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(query)) return;

            Console.WriteLine("\nПоиск...");

            try
            {
                var repositories = await _service.SearchRepositoriesAsync(
                    query, _settingsService.CurrentSettings.CountPage);

                if (repositories.Count == 0)
                {
                    ConsoleHelper.Pause("Репозитории не найдены.");
                    return;
                }

                Console.WriteLine($"\nНайдено: {repositories.Count} репозиториев.\n");

                for (int i = 0; i < repositories.Count; i++)
                {
                    GitHubRepository repo = repositories[i];
                    Console.WriteLine($"{i + 1}. {repo.Name} ({repo.Stars} stars) [{repo.Language ?? "N/A"}]");
                    if (!string.IsNullOrWhiteSpace(repo.Description))
                        Console.WriteLine($"   {repo.Description}");
                }

                Console.WriteLine();

                if (ConsoleHelper.AskYesNo("Сохранить в базу данных?"))
                {
                    int saved = 0;
                    foreach (var repo in repositories)
                    {
                        if (!_db.GitHubRepos.Any(r => r.Id == repo.Id))
                        {
                            _db.GitHubRepos.Add(repo);
                            saved++;
                        }
                    }
                    _db.SaveChanges();
                    Console.WriteLine($"Сохранено новых записей: {saved}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОшибка: {ex.Message}");
            }

            ConsoleHelper.Pause();
        }

        private void ShowSaved()
        {
            while (true)
            {
                ConsoleHelper.PrintHeader("Сохраненные репозитории");
                Console.WriteLine($"Всего записей: {_db.GitHubRepos.Count()}\n");

                var repositories = _db.GitHubRepos
                    .OrderByDescending(r => r.Stars)
                    .Take(50)
                    .ToList();

                if (repositories.Count == 0)
                {
                    ConsoleHelper.Pause("База данных пуста.");
                    return;
                }

                for (int i = 0; i < repositories.Count; i++)
                {
                    var repo = repositories[i];
                    Console.WriteLine($"{i + 1}. {repo.Name} ({repo.Stars} stars) [{repo.Language ?? "N/A"}]");
                }

                Console.Write("\nВыберите номер для просмотра (0 - назад): ");

                if (int.TryParse(Console.ReadLine(), out int index))
                {
                    if (index == 0) return;
                    if (index > 0 && index <= repositories.Count)
                        ShowDetails(repositories[index - 1]);
                }
            }
        }

        private void ShowDetails(GitHubRepository repository)
        {
            ConsoleHelper.PrintHeader("Детальная информация");
            Console.WriteLine($"Название:  {repository.Name}");
            Console.WriteLine($"Описание:  {repository.Description}");
            Console.WriteLine($"Звезды:    {repository.Stars}");
            Console.WriteLine($"Язык:      {repository.Language ?? "Не указан"}");
            Console.WriteLine($"Владелец:  {repository.OwnerLogin}");
            Console.WriteLine($"Создан:    {repository.CreatedAt:dd.MM.yyyy}");
            Console.WriteLine($"URL:       {repository.Url}");
            Console.WriteLine($"ID:        {repository.Id}");

            Console.WriteLine("\n1. Открыть в браузере");
            Console.WriteLine("2. Назад");
            Console.Write("Выберите: ");

            if (Console.ReadLine() == "1")
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
                    Console.WriteLine("Не удалось открыть браузер.");
                    ConsoleHelper.Pause();
                }
            }
        }
    }
}