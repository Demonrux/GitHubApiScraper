using System.Text.Json;
using InternetTechLab1.Models;

namespace InternetTechLab1.Services
{
    public class GitHubService
    {
        private readonly HttpClient _httpClient;

        public GitHubService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<GitHubRepository>> SearchRepositoriesAsync(string query , int perPage = 10)
        {  
            try
            {
                string url = $"https://api.github.com/search/repositories?q={Uri.EscapeDataString(query)}&per_page={perPage}";
                Console.WriteLine($"URL: {url}");
                Console.WriteLine($"Поиск репозиториев с: {query}");

                _httpClient.Timeout = TimeSpan.FromSeconds(30);

                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Ошибка API: {response.StatusCode}");

                    if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        Console.WriteLine("Лимит GitHub API");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    { 
                        Console.WriteLine("API не найден");
                    }

                    return new List<GitHubRepository>();
                }

                string json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Получен JSON, длина: {json.Length} символов \n");

                using JsonDocument document = JsonDocument.Parse(json);
                JsonElement root = document.RootElement;

                if (!root.TryGetProperty("items", out var items))
                {
                    Console.WriteLine("Элемены не найдены");
                    return new List<GitHubRepository>();
                }

                List<GitHubRepository> repositories = new List<GitHubRepository>();

                foreach (JsonElement item in items.EnumerateArray())
                {
                    try
                    {
                        GitHubRepository? repository = ParseRepository(item);
                        if (repository != null)
                        {
                            repositories.Add(repository);
                            Console.WriteLine($"Найден: {repository.Name} ({repository.Stars} stars)");
                        }
                    }
                    catch
                    {
                        // TODO
                    }
                }

                Console.WriteLine($"\nНайдено: {repositories.Count} репозиториев");
                return repositories;
            }

            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($" Ошибка сети: {httpEx.Message}");

                return new List<GitHubRepository>();
            }

            catch (TaskCanceledException)
            {
                Console.WriteLine("Проверьте подключение к интернету");
                return new List<GitHubRepository>();
            }

            catch (JsonException jsonEx)
            {
                Console.WriteLine($"Ошибка формата данных: {jsonEx.Message}");
                return new List<GitHubRepository>();
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                return new List<GitHubRepository>();
            }
        }

        private static GitHubRepository? ParseRepository(JsonElement item)
        {
            try
            {
                return new GitHubRepository
                {
                    Id = item.GetProperty("id").GetInt32(),
                    Name = item.GetProperty("name").GetString() ?? "",
                    Url = item.GetProperty("html_url").GetString() ?? "",
                    Description = item.GetProperty("description").GetString() ?? "",
                    OwnerLogin = item.GetProperty("owner").GetProperty("login").GetString() ?? "",
                    Language = item.TryGetProperty("language", out var lang) ? lang.GetString() : null,
                    Stars = item.GetProperty("stargazers_count").GetInt32(),
                    CreatedAt = DateTime.Parse(item.GetProperty("created_at").GetString() ?? "")
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка парсинга: {ex.Message}");
                return null;
            }
        }
    }
}