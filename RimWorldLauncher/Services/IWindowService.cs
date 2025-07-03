using System;
using System.Threading.Tasks;

namespace RimWorldLauncher.Services
{
    public interface IWindowService
    {
        void ShowSettings();
        void ShowLogs();
        void CloseWindow();
        void MinimizeWindow();
        void MaximizeWindow();
    }
} 