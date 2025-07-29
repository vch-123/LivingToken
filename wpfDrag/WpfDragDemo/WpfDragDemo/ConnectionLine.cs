//using System.Windows.Shapes;
//using System.Windows.Media;
//using System.Windows;

//namespace WpfDragDemo
//{
//    public class ConnectionLine
//    {
//        public NodeControl From { get; }
//        public NodeControl To { get; }
//        public Line Line { get; }

//        public ConnectionLine(NodeControl from, NodeControl to)
//        {
//            From = from;
//            To = to;

//            var p1 = from.GetOutputPosition();
//            var p2 = to.GetInputPosition();

//            Line = new Line
//            {
//                Stroke = Brushes.Black,
//                StrokeThickness = 2,
//                X1 = p1.X,
//                Y1 = p1.Y,
//                X2 = p2.X,
//                Y2 = p2.Y
//            };
//        }
//    }
//}
