using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RimWorldLauncher.Models;

namespace RimWorldLauncher.Services
{
    public interface IProcessService
    {
        event EventHandler<IEnumerable<ProcessInfo>>? ProcessesChanged;
        IEnumerable<ProcessInfo> RunningProcesses { get; }
        Task StartGameAsync(string gamePath, string arguments, int instanceCount);
        Task StopAllProcessesAsync();
        Task RefreshProcessesAsync();
        bool HasRunningProcesses { get; }
    }
} 