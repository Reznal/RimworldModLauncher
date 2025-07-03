using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using RimWorldLauncher.Models;

namespace RimWorldLauncher.Services
{
    public class LoggingService : ILoggingService
    {
        public ObservableCollection<LogEntry> Logs { get; } = new ObservableCollection<LogEntry>();

        public void Log(LogLevel level, string message)
        {
            var entry = new LogEntry(level, message);
            
            // Ensure we're on the UI thread when modifying the collection
            if (Application.Current?.Dispatcher.CheckAccess() == true)
            {
                Logs.Add(entry);
            }
            else
            {
                Application.Current?.Dispatcher.Invoke(() => Logs.Add(entry));
            }
        }

        public void LogDebug(string message) => Log(LogLevel.Debug, message);
        public void LogInfo(string message) => Log(LogLevel.Info, message);
        public void LogWarning(string message) => Log(LogLevel.Warning, message);
        public void LogError(string message) => Log(LogLevel.Error, message);

        public void Clear()
        {
            // Ensure we're on the UI thread when modifying the collection
            if (Application.Current?.Dispatcher.CheckAccess() == true)
            {
                Logs.Clear();
            }
            else
            {
                Application.Current?.Dispatcher.Invoke(() => Logs.Clear());
            }
        }

        public IEnumerable<LogEntry> GetFilteredLogs(LogLevel? level = null)
        {
            return level.HasValue ? Logs.Where(l => l.Level == level.Value) : Logs;
        }
    }
} 