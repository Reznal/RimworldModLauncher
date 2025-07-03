using System;
using System.Threading.Tasks;
using RimWorldLauncher.Models;

namespace RimWorldLauncher.Services
{
    public interface ISettingsService
    {
        Settings Settings { get; }
        Task LoadSettingsAsync();
        Task SaveSettingsAsync();
        string GetSettingsFilePath();
    }
} 