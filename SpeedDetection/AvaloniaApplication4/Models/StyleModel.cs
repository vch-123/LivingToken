using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication4.Models
{
    public class StyleModel
    {
    }

    public class ColorScheme
    {
        public string Name { get; set; } = "";
        public string SidebarBackground { get; set; } = "";
        public string ContentBackground { get; set; } = "";
        public string Foreground { get; set; } = "#CFD8DC"; // 文字颜色默认莫兰迪浅灰
    }

}
