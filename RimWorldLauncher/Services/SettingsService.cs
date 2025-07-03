using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using RimWorldLauncher.Models;

namespace RimWorldLauncher.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly string _settingsFilePath;

        public SettingsService()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appDataPath, "RimWorldLauncher");
            Directory.CreateDirectory(appFolder);
            _settingsFilePath = Path.Combine(appFolder, "settings.json");
        }

        public Settings Settings { get; private set; } = new Settings();

        public string GetSettingsFilePath() => _settingsFilePath;

        public async Task LoadSettingsAsync()
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    var json = await File.ReadAllTextAsync(_settingsFilePath);
                    Settings = JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
                }
            }
            catch (Exception)
            {
                Settings = new Settings();
            }
        }

        public async Task SaveSettingsAsync()
        {
            try
            {
                var json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(_settingsFilePath, json);
            }
            catch (Exception)
            {
                // Log error if logging service is available
            }
        }
    }
} 