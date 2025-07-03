using System.Windows;
using RimWorldLauncher.ViewModels;

namespace RimWorldLauncher.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
} 