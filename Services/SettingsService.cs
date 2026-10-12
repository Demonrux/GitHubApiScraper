using System.Text.Json;
using GitHubApiScraper.Models;

namespace GitHubApiScraper.Services
{
    public class SettingsService
    {
        private readonly string _settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

        public Setting CurrentSettings { get; private set; }

        public SettingsService()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    string json = File.ReadAllText(_settingsPath);
                    CurrentSettings = JsonSerializer.Deserialize<Setting>(json) ?? new Setting();
                    Console.WriteLine("Настройки успешно загружены.");
                }
                else
                {
                    CurrentSettings = new Setting();
                    SaveSettings();
                    Console.WriteLine("Создан файл настроек по умолчанию.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки настроек: {ex.Message}");
                CurrentSettings = new Setting();
            }
        }

        public void SaveSettings()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(CurrentSettings, options);
                File.WriteAllText(_settingsPath, json);
                Console.WriteLine("Настройки сохранены.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения настроек: {ex.Message}");
            }
        }

        public void UpdateSettings(Action<Setting> updateAction)
        {
            updateAction(CurrentSettings);
            SaveSettings();
        }
    }
}