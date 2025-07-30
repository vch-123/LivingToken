using Avalonia.Controls;
using AvaloniaApplication4.ViewModels;

namespace AvaloniaApplication4.Views
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent(); // 这个方法由 Avalonia 编译器自动生成
            DataContext = new SettingsViewModel();
        }
    }
}
