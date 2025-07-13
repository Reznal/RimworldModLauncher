namespace RimWorldLauncher.Models
{
    public class Settings
    {
        public string GamePath { get; set; } = string.Empty;
        public string ModPath { get; set; } = string.Empty;
        public string LaunchArguments { get; set; } = string.Empty;
        public bool AlwaysOnTop { get; set; } = false;
        public bool LogAllInstances { get; set; } = true;
        public int InstanceCount { get; set; } = 1;
        public bool ShowDebug { get; set; } = true;
        public bool ShowInfo { get; set; } = true;
        public bool ShowWarning { get; set; } = true;
        public bool ShowError { get; set; } = true;
    }
} 