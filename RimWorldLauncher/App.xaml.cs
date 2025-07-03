using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RimWorldLauncher.Services;
using RimWorldLauncher.ViewModels;
using RimWorldLauncher.Windows;
using System.Windows;
using Wpf.Ui;

namespace RimWorldLauncher
{
    public partial class App : Application
    {
        private IHost? _host;

        protected override async void OnStartup(StartupEventArgs e)
        {
            _host = CreateHostBuilder(e.Args).Build();
            await _host.StartAsync();

            // Load settings on startup
            var settingsService = _host.Services.GetRequiredService<ISettingsService>();
            await settingsService.LoadSettingsAsync();

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (_host != null)
            {
                await _host.StopAsync();
                _host.Dispose();
            }

            base.OnExit(e);
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // Register services
                    services.AddSingleton<ISettingsService, SettingsService>();
                    services.AddSingleton<IProcessService, ProcessService>();
                    services.AddSingleton<IBuildService, BuildService>();
                    services.AddSingleton<ILoggingService, LoggingService>();

                    // Register ViewModels
                    services.AddTransient<MainViewModel>();
                    services.AddTransient<LogsViewModel>();

                    // Register Windows
                    services.AddTransient<MainWindow>();
                    services.AddTransient<SettingsWindow>();
                    services.AddTransient<LogsWindow>();
                });
    }
} 