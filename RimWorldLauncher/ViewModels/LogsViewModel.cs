using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using RimWorldLauncher.Models;
using RimWorldLauncher.Services;

namespace RimWorldLauncher.ViewModels
{
    public class LogsViewModel : ViewModelBase
    {
        private readonly ILoggingService _loggingService;
        private readonly ISettingsService _settingsService;
        private bool _showDebug = true, _showInfo = true, _showWarning = true, _showError = true;
        public ObservableCollection<LogEntry> Logs => _loggingService.Logs;

        public bool ShowDebug { get => _showDebug; set { if (SetProperty(ref _showDebug, value)) { SaveFilter(); OnPropertyChanged(nameof(FilteredLogs)); } } }
        public bool ShowInfo { get => _showInfo; set { if (SetProperty(ref _showInfo, value)) { SaveFilter(); OnPropertyChanged(nameof(FilteredLogs)); } } }
        public bool ShowWarning { get => _showWarning; set { if (SetProperty(ref _showWarning, value)) { SaveFilter(); OnPropertyChanged(nameof(FilteredLogs)); } } }
        public bool ShowError { get => _showError; set { if (SetProperty(ref _showError, value)) { SaveFilter(); OnPropertyChanged(nameof(FilteredLogs)); } } }

        public IEnumerable<LogEntry> FilteredLogs
        {
            get
            {
                return Logs.Where(l =>
                    (ShowDebug && l.Level == LogLevel.Debug) ||
                    (ShowInfo && l.Level == LogLevel.Info) ||
                    (ShowWarning && l.Level == LogLevel.Warning) ||
                    (ShowError && l.Level == LogLevel.Error));
            }
        }

        public bool AlwaysOnTop => _settingsService.Settings.AlwaysOnTop;

        public ICommand ClearCommand { get; }
        public ICommand ClearFilterCommand { get; }
        public ICommand MaximizeCommand { get; }
        public ICommand CloseCommand { get; }

        public LogsViewModel(ILoggingService loggingService, ISettingsService settingsService)
        {
            _loggingService = loggingService;
            _settingsService = settingsService;
            
            ClearCommand = new RelayCommand(Clear);
            ClearFilterCommand = new RelayCommand(ClearFilter);
            MaximizeCommand = new RelayCommand(Maximize);
            CloseCommand = new RelayCommand(Close);
            
            // Initialize filter options
            _showDebug = _settingsService.Settings.ShowDebug;
            _showInfo = _settingsService.Settings.ShowInfo;
            _showWarning = _settingsService.Settings.ShowWarning;
            _showError = _settingsService.Settings.ShowError;
            
            // Subscribe to log changes
            _loggingService.Logs.CollectionChanged += (sender, e) => OnPropertyChanged(nameof(FilteredLogs));
        }

        private void Clear()
        {
            _loggingService.Clear();
            OnPropertyChanged(nameof(FilteredLogs));
        }

        private void ClearFilter()
        {
            ShowDebug = ShowInfo = ShowWarning = ShowError = true;
            SaveFilter();
        }

        private void SaveFilter() {
            _settingsService.Settings.ShowDebug = ShowDebug;
            _settingsService.Settings.ShowInfo = ShowInfo;
            _settingsService.Settings.ShowWarning = ShowWarning;
            _settingsService.Settings.ShowError = ShowError;
            _settingsService.SaveSettingsAsync();
        }

        private void Maximize()
        {
            // Find the logs window and toggle maximize state
            var logsWindow = System.Windows.Application.Current.Windows
                .OfType<RimWorldLauncher.Windows.LogsWindow>()
                .FirstOrDefault();
            
            if (logsWindow != null)
            {
                logsWindow.WindowState = logsWindow.WindowState == System.Windows.WindowState.Maximized 
                    ? System.Windows.WindowState.Normal 
                    : System.Windows.WindowState.Maximized;
            }
        }

        private void Close()
        {
            // Find the logs window and close it
            var logsWindow = System.Windows.Application.Current.Windows
                .OfType<RimWorldLauncher.Windows.LogsWindow>()
                .FirstOrDefault();
            
            logsWindow?.Close();
        }
    }
} 