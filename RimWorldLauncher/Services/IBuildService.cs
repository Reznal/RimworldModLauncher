using System;
using System.Threading.Tasks;

namespace RimWorldLauncher.Services
{
    public interface IBuildService
    {
        event EventHandler<string>? BuildOutputReceived;
        event EventHandler<bool>? BuildCompleted;
        event EventHandler<int>? BuildProgressChanged;
        
        bool IsBuilding { get; }
        Task<bool> BuildModAsync(string modPath);
    }
} 