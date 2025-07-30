using Avalonia.Controls;
using AvaloniaApplication4.Models;
using AvaloniaApplication4.Service;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AvaloniaApplication4.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ColorScheme> ColorSchemes { get; } = new();

        private ColorScheme _selectedColorScheme;
        public ColorScheme SelectedColorScheme
        {
            get => _selectedColorScheme;
            set => SetProperty(ref _selectedColorScheme, value);
        }

        public ObservableCollection<string> MenuItems { get; } = new()
        {
            "静态示例",
            "设备实时数据",
            "读取日志",
            "读取日志(独立曲线)",
            "配置"
        };

        private string? selectedMenuItem;
        public string? SelectedMenuItem
        {
            get => selectedMenuItem;
            set
            {
                if (selectedMenuItem != value)
                {
                    selectedMenuItem = value;
                    OnPropertyChanged();
                    UpdateSelectedView();
                }
            }
        }

        private Control? selectedView;
        public Control? SelectedView
        {
            get => selectedView;
            set
            {
                if (selectedView != value)
                {
                    selectedView = value;
                    OnPropertyChanged();
                }
            }
        }
        private readonly AppSettingsService _appSettingsService;

        public string SidebarBackground => _appSettingsService.SidebarBackground;
        public string ContentBackground => _appSettingsService.ContentBackground;
        public string Foreground => _appSettingsService.Foreground;

        public MainWindowViewModel()
        {
            _appSettingsService=App.Services.GetRequiredService<AppSettingsService>();
            // 绑定全局设置颜色
            _appSettingsService.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(AppSettingsService.SidebarBackground) ||
                    e.PropertyName == nameof(AppSettingsService.ContentBackground) ||
                    e.PropertyName == nameof(AppSettingsService.Foreground))
                {
                    OnPropertyChanged(nameof(SidebarBackground));
                    OnPropertyChanged(nameof(ContentBackground));
                    OnPropertyChanged(nameof(Foreground));
                }
            };             
            SelectedMenuItem = MenuItems[0];
        }

        private void UpdateSelectedView()
        {
            SelectedView = SelectedMenuItem switch
            {
                "静态示例" => new Views.HomeView(),
                "设备实时数据" => new Views.TestView(),
                "读取日志" => new Views.LogFuncView(),
                "读取日志(独立曲线)"=>new Views.LogThreeView(),
                "配置" => new Views.SettingsView(),
                _ => null
            };
        }

        // 这是缺失的 SetProperty 方法实现
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
