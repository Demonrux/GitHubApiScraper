using System.Text.Json.Serialization;

namespace InternetTechLab1.Models
{
    public class Setting
    {
        [JsonPropertyName("database")]
        public string Database { get; set; } = "InternetTechLab1.db";

        [JsonPropertyName("count_page")]
        public int CountPage { get; set; } = 10;

        [JsonPropertyName("timeout")]
        public int Timeout { get; set; } = 30;
    }
}