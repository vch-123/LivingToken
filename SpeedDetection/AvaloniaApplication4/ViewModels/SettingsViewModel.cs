using AvaloniaApplication4.Models;
using AvaloniaApplication4.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Linq;

namespace AvaloniaApplication4.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly AppSettingsService _appSettingsService;

        public ObservableCollection<ColorScheme> ColorSchemes { get; } = new();

        [ObservableProperty]
        private ColorScheme selectedColorScheme;

        public SettingsViewModel()
        {
            _appSettingsService = App.Services.GetRequiredService<AppSettingsService>();

            ColorSchemes.Add(new ColorScheme
            {
                Name = "素白",
                SidebarBackground = "#F7F7F7",  // 极浅灰白
                ContentBackground = "#FFFFFF",  // 纯白
                Foreground = "#2C2C2C"          // 深灰黑，保证对比度
            });

            ColorSchemes.Add(new ColorScheme
            {
                Name = "青灰石蓝",
                SidebarBackground = "#6B7B8C",
                ContentBackground = "#CFD8DC",
                Foreground = "#CFD8DC"
            });

            ColorSchemes.Add(new ColorScheme
            {
                Name = "暖沙褐",
                SidebarBackground = "#807060",
                ContentBackground = "#E0D9C6",
                Foreground = "#E0D9C6"
            });

            ColorSchemes.Add(new ColorScheme
            {
                Name = "灰棕蓝灰",
                SidebarBackground = "#8B8C89",
                ContentBackground = "#D9D6CF",
                Foreground = "#D9D6CF"
            });

            ColorSchemes.Add(new ColorScheme
            {
                Name = "暗夜墨蓝",
                SidebarBackground = "#1C1F26",   // 墨蓝偏黑
                ContentBackground = "#2B2F36",   // 深灰蓝
                Foreground = "#D6D6D6"           // 柔和灰白文字
            });
            ColorSchemes.Add(new ColorScheme
            {
                Name = "蜜桃绒暖",
                SidebarBackground = "#FFD1C1",
                ContentBackground = "#FFF3ED",
                Foreground = "#5A3E36"
            });

            
            ColorSchemes.Add(new ColorScheme
            {
                Name = "宁静玫瑰",
                SidebarBackground = "#AEC6CF",
                ContentBackground = "#F7DDE3",
                Foreground = "#2B2B2B"
            });

            ColorSchemes.Add(new ColorScheme
            {
                Name = "低饱绿茶",
                SidebarBackground = "#A8BBA2",
                ContentBackground = "#E5ECE2",
                Foreground = "#313D2F"
            });


            // 当前配色映射为已知预设的其中之一（否则默认第一个）
            SelectedColorScheme = ColorSchemes.FirstOrDefault(c =>
                c.SidebarBackground == _appSettingsService.SidebarBackground &&
                c.ContentBackground == _appSettingsService.ContentBackground &&
                c.Foreground == _appSettingsService.Foreground
            ) ?? ColorSchemes[0];

            ApplyColorScheme(SelectedColorScheme);
        }

        partial void OnSelectedColorSchemeChanged(ColorScheme value)
        {
            ApplyColorScheme(value);
        }

        private void ApplyColorScheme(ColorScheme scheme)
        {
            _appSettingsService.SidebarBackground = scheme.SidebarBackground;
            _appSettingsService.ContentBackground = scheme.ContentBackground;
            _appSettingsService.Foreground = scheme.Foreground;
        }

        public bool UseAbsoluteValue
        {
            get => _appSettingsService.UseAbsoluteValue;
            set
            {
                if (_appSettingsService.UseAbsoluteValue != value)
                {
                    _appSettingsService.UseAbsoluteValue = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
