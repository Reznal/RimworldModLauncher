using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using RimWorldLauncher.Models;

namespace RimWorldLauncher.Services
{
    public class ProcessService : IProcessService
    {
        private readonly List<ProcessInfo> _runningProcesses = new();
        private readonly object _lockObject = new();

        public event EventHandler<IEnumerable<ProcessInfo>>? ProcessesChanged;

        public IEnumerable<ProcessInfo> RunningProcesses
        {
            get
            {
                lock (_lockObject)
                {
                    return _runningProcesses.ToList();
                }
            }
        }

        public bool HasRunningProcesses
        {
            get
            {
                lock (_lockObject)
                {
                    return _runningProcesses.Any(p => p.IsRunning);
                }
            }
        }

        public async Task StartGameAsync(string gamePath, string arguments, int instanceCount)
        {
            for (int i = 0; i < instanceCount; i++)
            {
                try
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = gamePath,
                        Arguments = arguments,
                        UseShellExecute = true
                    };

                    var process = Process.Start(startInfo);
                    if (process != null)
                    {
                        var processInfo = new ProcessInfo(process);
                        lock (_lockObject)
                        {
                            _runningProcesses.Add(processInfo);
                        }
                        OnProcessesChanged();
                    }
                }
                catch (Exception)
                {
                    // Log error
                }
            }
        }

        public async Task StopAllProcessesAsync()
        {
            lock (_lockObject)
            {
                foreach (var processInfo in _runningProcesses.Where(p => p.IsRunning))
                {
                    try
                    {
                        processInfo.Process?.Kill();
                    }
                    catch (Exception)
                    {
                        // Log error
                    }
                }
                _runningProcesses.Clear();
            }
            OnProcessesChanged();
        }

        public async Task RefreshProcessesAsync()
        {
            lock (_lockObject)
            {
                _runningProcesses.RemoveAll(p => !p.IsRunning);
            }
            OnProcessesChanged();
        }

        private void OnProcessesChanged()
        {
            ProcessesChanged?.Invoke(this, RunningProcesses);
        }
    }
} 