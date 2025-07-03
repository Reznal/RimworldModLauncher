using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using RimWorldLauncher.Models;
using RimWorldLauncher.Services;
using RimWorldLauncher.Windows;

namespace RimWorldLauncher.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly IProcessService _processService;
        private readonly IBuildService _buildService;
        private readonly ILoggingService _loggingService;
        private int _instanceCount = 1;
        private string _statusText = "Ready";
        private bool _isStatusVisible = false;
        private double _progressValue = 0;
        private bool _isProgressVisible = false;
        private BuildStatus _buildStatus = BuildStatus.Idle;

        public MainViewModel(
            ISettingsService settingsService,
            IProcessService processService,
            IBuildService buildService,
            ILoggingService loggingService)
        {
            _settingsService = settingsService;
            _processService = processService;
            _buildService = buildService;
            _loggingService = loggingService;

            // Initialize commands
            IncrementInstanceCommand = new RelayCommand(IncrementInstance, () => InstanceCount < 10);
            DecrementInstanceCommand = new RelayCommand(DecrementInstance, () => InstanceCount > 1);
            BuildCommand = new RelayCommand(async () => await BuildAsync(), () => !_buildService.IsBuilding);
            RunCommand = new RelayCommand(async () => await RunAsync(), () => !string.IsNullOrEmpty(_settingsService.Settings.GamePath));
            TerminateCommand = new RelayCommand(async () => await TerminateAsync(), () => HasRunningProcesses);
            BuildAndRunCommand = new RelayCommand(async () => await BuildAndRunAsync(), () => !_buildService.IsBuilding && !string.IsNullOrEmpty(_settingsService.Settings.GamePath));
            OpenSettingsCommand = new RelayCommand(OpenSettings);
            OpenLogsCommand = new RelayCommand(OpenLogs);
            MinimizeCommand = new RelayCommand(Minimize);
            CloseCommand = new RelayCommand(Close);

            // Subscribe to events
            _processService.ProcessesChanged += OnProcessesChanged;
            _buildService.BuildOutputReceived += OnBuildOutputReceived;
            _buildService.BuildCompleted += OnBuildCompleted;
            _buildService.BuildProgressChanged += OnBuildProgressChanged;

            // Load initial settings
            LoadSettings();
        }

        public int InstanceCount
        {
            get => _instanceCount;
            set
            {
                if (SetProperty(ref _instanceCount, Math.Clamp(value, 1, 10)))
                {
                    SaveSettings();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        public bool IsStatusVisible
        {
            get => _isStatusVisible;
            set => SetProperty(ref _isStatusVisible, value);
        }

        public double ProgressValue
        {
            get => _progressValue;
            set => SetProperty(ref _progressValue, value);
        }

        public bool IsProgressVisible
        {
            get => _isProgressVisible;
            set => SetProperty(ref _isProgressVisible, value);
        }

        public bool HasRunningProcesses => _processService.HasRunningProcesses;

        public string RunButtonText => HasRunningProcesses ? "Restart" : "Run";
        public string BuildAndRunButtonText => HasRunningProcesses ? "Build & Restart" : "Build & Run";

        public bool AlwaysOnTop => _settingsService.Settings.AlwaysOnTop;

        public BuildStatus BuildStatus
        {
            get => _buildStatus;
            set { if (SetProperty(ref _buildStatus, value)) OnPropertyChanged(nameof(BuildButtonColor)); }
        }
        public string BuildButtonColor => BuildStatus == BuildStatus.Success ? "LimeGreen" : BuildStatus == BuildStatus.Fail ? "Red" : "DodgerBlue";

        public ICommand IncrementInstanceCommand { get; }
        public ICommand DecrementInstanceCommand { get; }
        public ICommand BuildCommand { get; }
        public ICommand RunCommand { get; }
        public ICommand TerminateCommand { get; }
        public ICommand BuildAndRunCommand { get; }
        public ICommand OpenSettingsCommand { get; }
        public ICommand OpenLogsCommand { get; }
        public ICommand MinimizeCommand { get; }
        public ICommand CloseCommand { get; }

        private void IncrementInstance()
        {
            InstanceCount++;
        }

        private void DecrementInstance()
        {
            InstanceCount--;
        }

        private async Task BuildAsync()
        {
            if (string.IsNullOrEmpty(_settingsService.Settings.ModPath))
            {
                _loggingService.LogWarning("Mod path not set. Please configure in Settings.");
                StatusText = "Mod path not configured";
                IsStatusVisible = true;
                await Task.Delay(3000);
                IsStatusVisible = false;
                return;
            }

            StatusText = "Building mod...";
            IsStatusVisible = true;
            IsProgressVisible = true;
            ProgressValue = 0;

            _loggingService.LogInfo($"Starting build for mod path: {_settingsService.Settings.ModPath}");

            BuildStatus = BuildStatus.Idle;
            var success = await _buildService.BuildModAsync(_settingsService.Settings.ModPath);
            BuildStatus = success ? BuildStatus.Success : BuildStatus.Fail;
            
            if (success)
            {
                StatusText = "Build successful";
                _loggingService.LogInfo("Mod build completed successfully");
            }
            else
            {
                StatusText = "Build failed";
                _loggingService.LogError("Mod build failed - check the logs window for detailed error information");
            }

            await Task.Delay(3000);
            IsStatusVisible = false;
            IsProgressVisible = false;
        }

        private async Task RunAsync()
        {
            if (string.IsNullOrEmpty(_settingsService.Settings.GamePath))
            {
                _loggingService.LogWarning("Game path not set. Please configure in Settings.");
                StatusText = "Game path not configured";
                IsStatusVisible = true;
                await Task.Delay(3000);
                IsStatusVisible = false;
                return;
            }

            // If instances are already running, restart them
            if (HasRunningProcesses)
            {
                StatusText = "Restarting instances...";
                IsStatusVisible = true;
                
                await _processService.StopAllProcessesAsync();
                await Task.Delay(1000); // Give time for processes to close
                
                _loggingService.LogInfo("Restarting game instances");
            }
            else
            {
                StatusText = $"Starting {InstanceCount} instance(s)...";
                IsStatusVisible = true;
                _loggingService.LogInfo($"Starting {InstanceCount} game instance(s)");
            }

            await _processService.StartGameAsync(
                _settingsService.Settings.GamePath,
                _settingsService.Settings.LaunchArguments,
                InstanceCount);

            StatusText = $"Started {InstanceCount} instance(s)";
            _loggingService.LogInfo($"Started {InstanceCount} game instance(s)");

            await Task.Delay(3000);
            IsStatusVisible = false;
        }

        private async Task TerminateAsync()
        {
            StatusText = "Terminating instances...";
            IsStatusVisible = true;

            await _processService.StopAllProcessesAsync();

            StatusText = "All instances stopped.";
            _loggingService.LogInfo("All game instances terminated");

            await Task.Delay(3000);
            IsStatusVisible = false;
        }

        private async Task BuildAndRunAsync()
        {
            if (string.IsNullOrEmpty(_settingsService.Settings.ModPath))
            {
                _loggingService.LogWarning("Mod path not set. Please configure in Settings.");
                StatusText = "Mod path not configured";
                IsStatusVisible = true;
                await Task.Delay(3000);
                IsStatusVisible = false;
                return;
            }

            StatusText = "Building and running...";
            IsStatusVisible = true;
            IsProgressVisible = true;
            ProgressValue = 0;

            _loggingService.LogInfo("Starting Build & Run process");

            BuildStatus = BuildStatus.Idle;
            var buildSuccess = await _buildService.BuildModAsync(_settingsService.Settings.ModPath);
            BuildStatus = buildSuccess ? BuildStatus.Success : BuildStatus.Fail;
            if (buildSuccess)
            {
                _loggingService.LogInfo("Build successful, starting game instances");
                await RunAsync();
            }
            else
            {
                StatusText = "Build failed - cannot run game";
                _loggingService.LogError("Build failed during Build & Run - game instances not started");
                await Task.Delay(3000);
                IsStatusVisible = false;
                IsProgressVisible = false;
            }
        }

        private void OnProcessesChanged(object? sender, IEnumerable<ProcessInfo> processes)
        {
            OnPropertyChanged(nameof(HasRunningProcesses));
            OnPropertyChanged(nameof(RunButtonText));
            OnPropertyChanged(nameof(BuildAndRunButtonText));
        }

        private void OnBuildOutputReceived(object? sender, string output)
        {
            if (string.IsNullOrWhiteSpace(output)) return;
            var lower = output.ToLowerInvariant();

            if (lower.Contains("warning"))
                _loggingService.LogWarning(output);
            else if (lower.Contains("error"))
                _loggingService.LogError(output);
            else
                _loggingService.LogInfo(output);
        }

        private void OnBuildCompleted(object? sender, bool success)
        {
            ProgressValue = 100;
            BuildStatus = success ? BuildStatus.Success : BuildStatus.Fail;
            CommandManager.InvalidateRequerySuggested();
        }

        private void OnBuildProgressChanged(object? sender, int progress)
        {
            ProgressValue = progress;
        }

        private void LoadSettings()
        {
            InstanceCount = _settingsService.Settings.InstanceCount;
        }

        private void SaveSettings()
        {
            _settingsService.Settings.InstanceCount = InstanceCount;
            _ = _settingsService.SaveSettingsAsync();
        }

        private void OpenSettings()
        {
            var settingsWindow = new SettingsWindow(_settingsService);
            settingsWindow.ShowDialog();
            
            // Refresh properties that might have changed in settings
            OnPropertyChanged(nameof(AlwaysOnTop));
        }

        private void OpenLogs()
        {
            var logsViewModel = new LogsViewModel(_loggingService, _settingsService);
            var logsWindow = new LogsWindow(logsViewModel);
            logsWindow.Show();
        }

        private void Minimize()
        {
            System.Windows.Application.Current.MainWindow.WindowState = System.Windows.WindowState.Minimized;
        }

        private void Close()
        {
            System.Windows.Application.Current.Shutdown();
        }
    }

    public enum BuildStatus { Idle, Success, Fail }
} 