using System;
using System.Globalization;
using System.Windows.Data;
using RimWorldLauncher.Models;

namespace RimWorldLauncher.Converters
{
    public class LogLevelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "All";
            
            if (value is LogLevel level)
            {
                return level switch
                {
                    LogLevel.Debug => "Debug",
                    LogLevel.Info => "Info",
                    LogLevel.Warning => "Warning",
                    LogLevel.Error => "Error",
                    _ => level.ToString()
                };
            }
            return value?.ToString() ?? "All";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 