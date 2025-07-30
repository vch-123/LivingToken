using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication4.Models
{
    public class LogModel
    {
        public DateTime LogTime { get; set; }
        public string Info { get; set; }

        public string Display => $"{LogTime:HH:mm:ss} - {Info}";
    }

    public class Point
    {
        public int A { get; set; }
        public int B { get; set; }
        public string C { get; set; }  // C 是横坐标
    }
}
