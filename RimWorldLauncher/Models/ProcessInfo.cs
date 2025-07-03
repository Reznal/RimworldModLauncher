using System;
using System.Diagnostics;

namespace RimWorldLauncher.Models
{
    public class ProcessInfo
    {
        public int ProcessId { get; set; }
        public string ProcessName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public Process? Process { get; set; }

        public ProcessInfo(Process process)
        {
            ProcessId = process.Id;
            ProcessName = process.ProcessName;
            FilePath = process.MainModule?.FileName ?? string.Empty;
            StartTime = process.StartTime;
            Process = process;
        }

        public bool IsRunning => Process?.HasExited == false;
    }
} 