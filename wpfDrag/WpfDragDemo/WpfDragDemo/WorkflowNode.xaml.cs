using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfDragDemo
{
    public partial class WorkflowNode : UserControl
    {
        public WorkflowNode()
        {
            InitializeComponent();

            BtnDelete.Click += BtnDelete_Click;

            MouseLeftButtonDown += WorkflowNode_MouseLeftButtonDown;
            MouseMove += WorkflowNode_MouseMove;
            MouseLeftButtonUp += WorkflowNode_MouseLeftButtonUp;
        }

        public event EventHandler? DeleteRequested;
        public event EventHandler? PositionChanged;

        private bool isDragging = false;
        private Point mouseStartPoint;
        private Point elementStartPoint;

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            DeleteRequested?.Invoke(this, EventArgs.Empty);
        }

        private void WorkflowNode_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 点击圆点不拖拽
            var source = e.OriginalSource as FrameworkElement;
            if (source == InputPoint || source == OutputPoint)
                return;

            var canvas = Parent as Canvas;
            if (canvas == null) return;

            isDragging = true;
            mouseStartPoint = e.GetPosition(canvas);
            elementStartPoint = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
            CaptureMouse();
            e.Handled = true;
        }

        private void WorkflowNode_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDragging) return;

            var canvas = Parent as Canvas;
            if (canvas == null) return;

            Point currentPos = e.GetPosition(canvas);
            Vector offset = currentPos - mouseStartPoint;

            double newX = elementStartPoint.X + offset.X;
            double newY = elementStartPoint.Y + offset.Y;

            newX = Math.Max(0, Math.Min(canvas.ActualWidth - ActualWidth, newX));
            newY = Math.Max(0, Math.Min(canvas.ActualHeight - ActualHeight, newY));

            Canvas.SetLeft(this, newX);
            Canvas.SetTop(this, newY);

            PositionChanged?.Invoke(this, EventArgs.Empty);

            e.Handled = true;
        }

        private void WorkflowNode_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!isDragging) return;
            isDragging = false;
            ReleaseMouseCapture();
            e.Handled = true;
        }
    }
}
