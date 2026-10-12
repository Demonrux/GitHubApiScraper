using System.Text.Json;
using GitHubApiScraper.Models;

namespace GitHubApiScraper.Services
{
    public class GitHubService
    {
        private readonly HttpClient _httpClient;

        public GitHubService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<GitHubRepository>> SearchRepositoriesAsync(string query, int perPage = 10)
        {
            try
            {
                string url = $"https://api.github.com/search/repositories?q={Uri.EscapeDataString(query)}&per_page={perPage}";

                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Ошибка API: {response.StatusCode}");

                    if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        Console.WriteLine("Превышен лимит запросов к GitHub API.");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        Console.WriteLine("Ресурс API не найден.");
                    }

                    return new List<GitHubRepository>();
                }

                string json = await response.Content.ReadAsStringAsync();
                using JsonDocument document = JsonDocument.Parse(json);
                JsonElement root = document.RootElement;

                if (!root.TryGetProperty("items", out var items))
                {
                    Console.WriteLine("Элементы не найдены в ответе.");
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
                        }
                    }
                    catch
                    {
                        // skip
                    }
                }

                return repositories;
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Ошибка сети: {httpEx.Message}");
                return new List<GitHubRepository>();
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Превышено время ожидания ответа от сервера.");
                return new List<GitHubRepository>();
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"Ошибка формата данных: {jsonEx.Message}");
                return new List<GitHubRepository>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Непредвиденная ошибка: {ex.Message}");
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
            catch
            {
                return null;
            }
        }
    }
}