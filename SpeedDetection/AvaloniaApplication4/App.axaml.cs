using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using AvaloniaApplication4.Service;
using System;
using LiveChartsCore;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView;
using AvaloniaApplication4.Clients;
using Microsoft.Extensions.Configuration;

namespace AvaloniaApplication4
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = default!;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            LiveCharts.Configure(config => config.HasGlobalSKTypeface(SKFontManager.Default.MatchCharacter('汉')));
        }

        public override void OnFrameworkInitializationCompleted()
        {

            

            // 注册服务
            var services = new ServiceCollection();

            ConfigureServices(services);

            Services = services.BuildServiceProvider();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                

                Services.GetRequiredService<EquipDataService>();
                Services.GetRequiredService<AppSettingsService>();
                Services.GetRequiredService<ConfigService>();
                Services.GetRequiredService<MqttClient>();

                desktop.MainWindow = new Views.MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory) // 或 AppDomain.CurrentDomain.BaseDirectory
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();
            services.AddSingleton<IConfiguration>(configuration);
            // 注册为单例
            services.AddSingleton<EquipDataService>();
            services.AddSingleton<AppSettingsService>();
            services.AddSingleton<ConfigService>();
            services.AddSingleton<MqttClient>();
        }
    }
}
