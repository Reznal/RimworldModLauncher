using System.Windows;
using RimWorldLauncher.ViewModels;
using RimWorldLauncher.Services;

namespace RimWorldLauncher.Windows
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(ISettingsService settingsService)
        {
            InitializeComponent();
            var viewModel = new SettingsViewModel(settingsService, this);
            DataContext = viewModel;
        }
    }
} 