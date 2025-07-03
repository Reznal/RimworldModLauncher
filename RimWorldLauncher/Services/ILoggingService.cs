using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RimWorldLauncher.Models;

namespace RimWorldLauncher.Services
{
    public interface ILoggingService
    {
        ObservableCollection<LogEntry> Logs { get; }
        void Log(LogLevel level, string message);
        void LogDebug(string message);
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);
        void Clear();
        IEnumerable<LogEntry> GetFilteredLogs(LogLevel? level = null);
    }
} 