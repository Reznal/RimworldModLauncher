using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using RimWorldLauncher.Models;
using RimWorldLauncher.Services;

namespace RimWorldLauncher.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly Window _window;
        private string _gamePath = string.Empty;
        private string _modPath = string.Empty;
        private string _launchArguments = string.Empty;
        private bool _alwaysOnTop = false;
        private bool _logAllInstances = true;

        public SettingsViewModel(ISettingsService settingsService, Window window)
        {
            _settingsService = settingsService;
            _window = window;
            
            BrowseGamePathCommand = new RelayCommand(BrowseGamePath);
            BrowseModPathCommand = new RelayCommand(BrowseModPath);
            SaveCommand = new RelayCommand(async () => await SaveAsync());
            CloseCommand = new RelayCommand(Close);
            
            LoadSettings();
        }

        public string GamePath
        {
            get => _gamePath;
            set => SetProperty(ref _gamePath, value);
        }

        public string ModPath
        {
            get => _modPath;
            set => SetProperty(ref _modPath, value);
        }

        public string LaunchArguments
        {
            get => _launchArguments;
            set => SetProperty(ref _launchArguments, value);
        }

        public bool AlwaysOnTop
        {
            get => _alwaysOnTop;
            set => SetProperty(ref _alwaysOnTop, value);
        }

        public bool LogAllInstances
        {
            get => _logAllInstances;
            set => SetProperty(ref _logAllInstances, value);
        }

        public ICommand BrowseGamePathCommand { get; }
        public ICommand BrowseModPathCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CloseCommand { get; }

        private void BrowseGamePath()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select RimWorld Executable",
                Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*",
                InitialDirectory = GetInitialDirectory(GamePath)
            };

            if (dialog.ShowDialog() == true)
            {
                GamePath = dialog.FileName;
            }
        }

        private void BrowseModPath()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Mod Project File",
                Filter = "Project files (*.csproj;*.sln)|*.csproj;*.sln|All files (*.*)|*.*",
                InitialDirectory = GetInitialDirectory(ModPath, GamePath)
            };

            if (dialog.ShowDialog() == true)
            {
                ModPath = dialog.FileName;
            }
        }

        private string GetInitialDirectory(string currentPath, string fallbackPath = "")
        {
            if (!string.IsNullOrEmpty(currentPath) && File.Exists(currentPath))
            {
                return Path.GetDirectoryName(currentPath) ?? "";
            }
            
            if (!string.IsNullOrEmpty(fallbackPath) && File.Exists(fallbackPath))
            {
                return Path.GetDirectoryName(fallbackPath) ?? "";
            }

            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        private void LoadSettings()
        {
            GamePath = _settingsService.Settings.GamePath;
            ModPath = _settingsService.Settings.ModPath;
            LaunchArguments = _settingsService.Settings.LaunchArguments;
            AlwaysOnTop = _settingsService.Settings.AlwaysOnTop;
            LogAllInstances = _settingsService.Settings.LogAllInstances;
        }

        private async Task SaveAsync()
        {
            _settingsService.Settings.GamePath = GamePath;
            _settingsService.Settings.ModPath = ModPath;
            _settingsService.Settings.LaunchArguments = LaunchArguments;
            _settingsService.Settings.AlwaysOnTop = AlwaysOnTop;
            _settingsService.Settings.LogAllInstances = LogAllInstances;

            await _settingsService.SaveSettingsAsync();
        }

        private void Close()
        {
            _window.Close();
        }
    }
} 