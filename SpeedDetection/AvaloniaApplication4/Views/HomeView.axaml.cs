using Avalonia;
using Avalonia.Controls;
using AvaloniaApplication4.ViewModels;
using LiveChartsCore.SkiaSharpView.Avalonia;

namespace AvaloniaApplication4.Views
{
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
            var vm = new HomeViewModel();
            DataContext = vm;

           
        }
    }


}
