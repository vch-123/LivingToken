using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication4.Models
{
    internal class MainData
    {
        public DateTime Time { get; set; }
        public int PlcOnline { get; set; }
        public string DeviceName { get; set; }
        public string CMD { get; set; }
        public long CommID { get; set; }
        public decimal XS { get; set; }
        public decimal YS { get; set; }
        public decimal ZS { get; set; }
    }
}
