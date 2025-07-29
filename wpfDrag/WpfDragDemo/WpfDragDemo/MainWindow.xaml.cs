using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfDragDemo
{
    public partial class MainWindow : Window
    {
        private int nodeCount = 0;

        private List<WorkflowNode> nodes = new();
        private List<Line> lines = new();

        private WorkflowNode? lineStartNode = null;
        private Line? tempLine = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddNode_Click(object sender, RoutedEventArgs e)
        {
            var node = new WorkflowNode();
            node.TextBoxTitle.Text = $"节点 {++nodeCount}";

            Canvas.SetLeft(node, 50 + nodeCount * 20);
            Canvas.SetTop(node, 50 + nodeCount * 20);

            node.DeleteRequested += Node_DeleteRequested;
            node.PositionChanged += (s, args) => UpdateAllLines();

            MainCanvas.Children.Add(node);
            nodes.Add(node);
        }

        private void Node_DeleteRequested(object? sender, EventArgs e)
        {
            if (sender is not WorkflowNode node) return;

            // 删除相关连线
            var relatedLines = lines.Where(l =>
                l.Tag is Tuple<WorkflowNode, WorkflowNode> t &&
                (t.Item1 == node || t.Item2 == node)).ToList();

            foreach (var line in relatedLines)
            {
                MainCanvas.Children.Remove(line);
                lines.Remove(line);
            }

            // 删除节点
            MainCanvas.Children.Remove(node);
            nodes.Remove(node);
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePos = e.GetPosition(MainCanvas);

            // 判断是否按下在某个节点的输出点附近，开始连线
            foreach (var node in nodes)
            {
                Point outputPos = GetNodeOutputPoint(node);
                double distance = (outputPos - mousePos).Length;
                if (distance <= 10)
                {
                    lineStartNode = node;

                    tempLine = new Line()
                    {
                        Stroke = Brushes.Blue,
                        StrokeThickness = 2,
                        X1 = outputPos.X,
                        Y1 = outputPos.Y,
                        X2 = mousePos.X,
                        Y2 = mousePos.Y,
                        StrokeDashArray = new DoubleCollection() { 2, 2 }
                    };

                    MainCanvas.Children.Add(tempLine);
                    MainCanvas.CaptureMouse();

                    e.Handled = true;
                    break;
                }
            }
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (tempLine != null)
            {
                Point pos = e.GetPosition(MainCanvas);
                tempLine.X2 = pos.X;
                tempLine.Y2 = pos.Y;
            }

            UpdateAllLines();
        }

        private void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (tempLine == null)
                return;

            Point mousePos = e.GetPosition(MainCanvas);

            WorkflowNode? lineEndNode = null;
            foreach (var node in nodes)
            {
                Point inputPos = GetNodeInputPoint(node);
                double distance = (inputPos - mousePos).Length;
                if (distance <= 10)
                {
                    lineEndNode = node;
                    break;
                }
            }

            if (lineEndNode != null && lineStartNode != null && lineEndNode != lineStartNode)
            {
                var line = new Line()
                {
                    Stroke = Brushes.Blue,
                    StrokeThickness = 2,
                    X1 = GetNodeOutputPoint(lineStartNode).X,
                    Y1 = GetNodeOutputPoint(lineStartNode).Y,
                    X2 = GetNodeInputPoint(lineEndNode).X,
                    Y2 = GetNodeInputPoint(lineEndNode).Y,
                    Tag = new Tuple<WorkflowNode, WorkflowNode>(lineStartNode, lineEndNode)
                };

                lines.Add(line);
                MainCanvas.Children.Add(line);
            }

            MainCanvas.Children.Remove(tempLine);
            tempLine = null;
            lineStartNode = null;
            MainCanvas.ReleaseMouseCapture();

            e.Handled = true;
        }

        private Point GetNodeOutputPoint(WorkflowNode node)
        {
            return node.OutputPoint.TranslatePoint(new Point(node.OutputPoint.ActualWidth / 2, node.OutputPoint.ActualHeight / 2), MainCanvas);
        }

        private Point GetNodeInputPoint(WorkflowNode node)
        {
            return node.InputPoint.TranslatePoint(new Point(node.InputPoint.ActualWidth / 2, node.InputPoint.ActualHeight / 2), MainCanvas);
        }

        private void UpdateAllLines()
        {
            foreach (var line in lines)
            {
                if (line.Tag is Tuple<WorkflowNode, WorkflowNode> t)
                {
                    line.X1 = GetNodeOutputPoint(t.Item1).X;
                    line.Y1 = GetNodeOutputPoint(t.Item1).Y;
                    line.X2 = GetNodeInputPoint(t.Item2).X;
                    line.Y2 = GetNodeInputPoint(t.Item2).Y;
                }
            }
        }
    }
}
