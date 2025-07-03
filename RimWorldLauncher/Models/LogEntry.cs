using System;

namespace RimWorldLauncher.Models
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }

    public class LogEntry
    {
        public LogLevel Level { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public LogEntry(LogLevel level, string message)
        {
            Level = level;
            Message = message?.Trim() ?? string.Empty;
        }

        public override string ToString()
        {
            return $"[{Level}] {Message}";
        }

        public string LogLevelIcon => Level switch
        {
            LogLevel.Debug => "\uF188",   // bug
            LogLevel.Info => "\uF05A",    // info-circle
            LogLevel.Warning => "\uF071", // exclamation-triangle
            LogLevel.Error => "\uF06A",   // exclamation-circle
            _ => "\uF05A"                  // info-circle as default
        };

        public string DisplayMessage => $"[{Level}] {Message}";
    }
} 