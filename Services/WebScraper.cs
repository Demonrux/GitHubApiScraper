using HtmlAgilityPack;
using InternetTechLab1.Models;
using System.Net;
using System.Text.Json;

namespace InternetTechLab1.Services
{
    public class WebScraper
    {
        private readonly HttpClient _httpClient;

        public WebScraper()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0 Safari/537.36");
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<ScrapedPage?> ScrapeUrlAsync(string url)
        {
            try
            {
                Console.WriteLine($"Загрузка: {url}");

                string html = await _httpClient.GetStringAsync(url);
                Console.WriteLine($"Получено: {html.Length} символов");

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                ScrapedPage result = new ScrapedPage
                {
                    Url = url,
                    Title = ExtractTitle(htmlDoc),
                    H1 = ExtractH1(htmlDoc),
                    Description = ExtractMetaDescription(htmlDoc),
                    LinksJson = JsonSerializer.Serialize(ExtractLinks(htmlDoc)),
                    ContentPreview = ExtractFirstParagraph(htmlDoc),
                    ScrapedAt = DateTime.UtcNow
                };

                return result;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Ошибка сети: {ex.Message}");
                return null;
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine($"Превышено время ожидания");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                return null;
            }
        }

        private static string ExtractTitle(HtmlDocument document)
        {
            var titleNode = document.DocumentNode.SelectSingleNode("//title");
            return titleNode != null ? WebUtility.HtmlDecode(titleNode.InnerText.Trim()) : "Заголовок не найден";
        }

        private static string? ExtractH1(HtmlDocument document)
        {
            var h1Node = document.DocumentNode.SelectSingleNode("//h1");
            return h1Node != null ? WebUtility.HtmlDecode(h1Node.InnerText.Trim()) : null;
        }

        private static string? ExtractMetaDescription(HtmlDocument document)
        {
            var metaNode = document.DocumentNode.SelectSingleNode("//meta[@name='description']");
            if (metaNode != null)
            {
                var content = metaNode.GetAttributeValue("content", "");
                return !string.IsNullOrEmpty(content) ? WebUtility.HtmlDecode(content.Trim()) : null;
            }
            return null;
        }

        private static List<string> ExtractLinks(HtmlDocument document)
        {
            var links = new List<string>();
            var linkNodes = document.DocumentNode.SelectNodes("//a[@href]");

            if (linkNodes != null)
            {
                foreach (var node in linkNodes)
                {
                    string link = node.GetAttributeValue("href", "");

                    if (!string.IsNullOrWhiteSpace(link) &&
                        !link.StartsWith("#") &&
                        !link.StartsWith("javascript:"))
                    {
                        links.Add(WebUtility.HtmlDecode(link));
                    }
                }
            }

            return links;
        }

        private static string? ExtractFirstParagraph(HtmlDocument document)
        {
            var pNode = document.DocumentNode.SelectSingleNode("//p");

            if (pNode != null)
            {
                string text = pNode.InnerText.Trim();

                if (text.Length > 200)
                {
                    text = text.Substring(0, 200) + "...";
                }

                return WebUtility.HtmlDecode(text);
            }

            return null;
        }
    }
}