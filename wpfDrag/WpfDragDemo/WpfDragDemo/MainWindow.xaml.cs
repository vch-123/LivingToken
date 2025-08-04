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
        private List<Connection> connections = new();

        private WorkflowNode? lineStartNode = null;
        private Path? tempPath = null;

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
            node.PositionChanged += (s, args) => UpdateAllConnections();

            MainCanvas.Children.Add(node);
            nodes.Add(node);
        }

        private void Node_DeleteRequested(object? sender, EventArgs e)
        {
            if (sender is not WorkflowNode node) return;

            // 删除相关连线
            var relatedConnections = connections.Where(c =>
                c.StartNode == node || c.EndNode == node).ToList();

            foreach (var conn in relatedConnections)
            {
                MainCanvas.Children.Remove(conn.Path);
                connections.Remove(conn);
            }

            // 删除节点
            MainCanvas.Children.Remove(node);
            nodes.Remove(node);
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePos = e.GetPosition(MainCanvas);

            // 点击连接线外不处理临时连线，先隐藏右键菜单（若有）
            // 右键菜单自动关闭，这里不处理

            // 判断是否按下在某个节点的输出点附近，开始连线
            foreach (var node in nodes)
            {
                Point outputPos = GetNodeOutputPoint(node);
                double distance = (outputPos - mousePos).Length;
                if (distance <= 10)
                {
                    lineStartNode = node;

                    tempPath = CreateConnectionPath(outputPos, mousePos, dashed: true);
                    MainCanvas.Children.Add(tempPath);

                    MainCanvas.CaptureMouse();

                    e.Handled = true;
                    break;
                }
            }
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (tempPath != null && lineStartNode != null)
            {
                Point pos = e.GetPosition(MainCanvas);

                UpdateTempConnectionPath(tempPath, GetNodeOutputPoint(lineStartNode), pos);
            }
        }

        private void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (tempPath == null || lineStartNode == null)
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

            if (lineEndNode != null && lineEndNode != lineStartNode)
            {
                var path = CreateConnectionPath(GetNodeOutputPoint(lineStartNode), GetNodeInputPoint(lineEndNode));
                var conn = new Connection(lineStartNode, lineEndNode, path);
                connections.Add(conn);
                MainCanvas.Children.Add(path);
            }

            MainCanvas.Children.Remove(tempPath);
            tempPath = null;
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

        private Path CreateConnectionPath(Point start, Point end, bool dashed = false)
        {
            var path = new Path
            {
                Stroke = Brushes.Blue,
                StrokeThickness = 4,
                Cursor = Cursors.Hand
            };

            if (dashed)
                path.StrokeDashArray = new DoubleCollection() { 4, 4 };

            var geometry = new PathGeometry();
            var figure = new PathFigure { StartPoint = start };

            double offset = Math.Abs(end.X - start.X) / 2;
            var bezier = new BezierSegment
            {
                Point1 = new Point(start.X + offset, start.Y),
                Point2 = new Point(end.X - offset, end.Y),
                Point3 = end
            };
            figure.Segments.Add(bezier);
            geometry.Figures.Add(figure);

            var arrow = CreateArrowHead(end, bezier.Point2);

            var group = new GeometryGroup();
            group.Children.Add(geometry);
            group.Children.Add(arrow);

            path.Data = group;

            // 绑定右键菜单
            path.ContextMenu = CreateConnectionContextMenu(path);

            return path;
        }

        private void UpdateTempConnectionPath(Path path, Point start, Point end)
        {
            var geometryGroup = new GeometryGroup();

            var geometry = new PathGeometry();
            var figure = new PathFigure { StartPoint = start };

            double offset = Math.Abs(end.X - start.X) / 2;
            var bezier = new BezierSegment
            {
                Point1 = new Point(start.X + offset, start.Y),
                Point2 = new Point(end.X - offset, end.Y),
                Point3 = end
            };
            figure.Segments.Add(bezier);
            geometry.Figures.Add(figure);

            geometryGroup.Children.Add(geometry);
            geometryGroup.Children.Add(CreateArrowHead(end, bezier.Point2));

            path.Data = geometryGroup;
        }

        private Geometry CreateArrowHead(Point arrowTip, Point controlPoint)
        {
            Vector direction = arrowTip - controlPoint;
            direction.Normalize();
            Vector perpendicular = new(-direction.Y, direction.X);

            double arrowLength = 10;
            double arrowWidth = 5;

            Point p1 = arrowTip;
            Point p2 = arrowTip - direction * arrowLength + perpendicular * arrowWidth;
            Point p3 = arrowTip - direction * arrowLength - perpendicular * arrowWidth;

            var figure = new PathFigure { StartPoint = p1, IsClosed = true, IsFilled = true };
            figure.Segments.Add(new LineSegment(p2, true));
            figure.Segments.Add(new LineSegment(p3, true));

            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            return geometry;
        }

        private ContextMenu CreateConnectionContextMenu(Path path)
        {
            var contextMenu = new ContextMenu();

            var menuItemDelete = new MenuItem { Header = "删除连接" };
            menuItemDelete.Click += (s, e) =>
            {
                var connection = connections.FirstOrDefault(c => c.Path == path);
                if (connection != null)
                {
                    MainCanvas.Children.Remove(connection.Path);
                    connections.Remove(connection);
                }
            };

            contextMenu.Items.Add(menuItemDelete);
            return contextMenu;
        }

        private void UpdateAllConnections()
        {
            foreach (var conn in connections)
            {
                UpdateTempConnectionPath(conn.Path, GetNodeOutputPoint(conn.StartNode), GetNodeInputPoint(conn.EndNode));
            }
        }

        private class Connection
        {
            public WorkflowNode StartNode { get; }
            public WorkflowNode EndNode { get; }
            public Path Path { get; }

            public Connection(WorkflowNode start, WorkflowNode end, Path path)
            {
                StartNode = start;
                EndNode = end;
                Path = path;
            }
        }
    }
}
