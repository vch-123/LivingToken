using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaApplication4.ViewModels;
using System.Collections.Generic;

namespace AvaloniaApplication4.Views;

public partial class LogThreeView : UserControl
{
    private LogThreeViewModel ViewModel => (LogThreeViewModel)DataContext!;

    public LogThreeView()
    {
        InitializeComponent();
        DataContext = new LogThreeViewModel();
    }

    private async void OnSelectLogFileButtonClick(object? sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "选择要读取的日志文件",
            AllowMultiple = false,
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter { Name = "日志文件", Extensions = { "json" } },
                new FileDialogFilter { Name = "所有文件", Extensions = { "*" } }
            }
        };

        var result = await dialog.ShowAsync((Window)this.VisualRoot!);
        if (result != null && result.Length > 0)
        {
            ViewModel.LoadLogFile(result[0]);
        }
    }

    private async void OnSelectTaskLogFileButtonClick(object? sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "选择要读取的任务日志文件",
            AllowMultiple = false,
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter { Name = "任务日志文件", Extensions = { "log" } },
                new FileDialogFilter { Name = "所有文件", Extensions = { "*" } }
            }
        };

        var result = await dialog.ShowAsync((Window)this.VisualRoot!);
        if (result != null && result.Length > 0)
        {
            ViewModel.LoadTaskLogFile(result[0]);
        }
    }
    public async void OnDrawTaskClick(object? sender, RoutedEventArgs e)
    {
        ViewModel.DrawTaskChart();
    }

}