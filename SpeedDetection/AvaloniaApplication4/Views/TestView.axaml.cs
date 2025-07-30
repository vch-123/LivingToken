using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaApplication4.ViewModels;
using LiveChartsCore;

namespace AvaloniaApplication4.Views;

public partial class TestView : UserControl
{
    public TestView()
    {
        InitializeComponent();
        DataContext = new TestViewModel();  // 这里初始化绑定VM实例

        Chart.PointerMoved += Chart_PointerMoved;
        Chart.PointerPressed += Chart_PointerPressed;
    }

    // 事件代码同上...



    private void Chart_PointerMoved(object? sender, PointerEventArgs e)
    {
        var pos = e.GetPosition(Chart);
        if (DataContext is TestViewModel vm)
        {
            vm.UpdateHoverIndexFromPosition(pos.X, Chart.Bounds.Width);
        }
    }

    private void Chart_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var pos = e.GetPosition(Chart);
        if (DataContext is TestViewModel vm)
        {
            vm.SelectPointFromPosition(pos.X, Chart.Bounds.Width);
        }
    }

    private void UpdateNoteButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is TestViewModel vm)
        {
            // 取TextBox输入的备注文本
            var newNote = textBox_updateNote.Text ?? string.Empty;

            // 调用ViewModel的更新方法，传入备注
            vm.UpdateNoteWithText(newNote);
        }
    }

}
