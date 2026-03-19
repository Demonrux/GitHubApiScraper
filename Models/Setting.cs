using System.Text.Json.Serialization;

public class Setting
{
    [JsonPropertyName("gitHubReposDatabase")]
    public string GitHubReposDatabase { get; set; } = "gitHubRepos.db";

    [JsonPropertyName("scrapingDatabase")]
    public string ScrapingDatabase { get; set; } = "scraper.db";

    [JsonPropertyName("count_page")]
    public int CountPage { get; set; } = 10;

    [JsonPropertyName("timeout")]
    public int Timeout { get; set; } = 30;
}