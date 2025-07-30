using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication4.Models
{
    public class CraneSpeedModel
    {
        public double XS { get; set; }//小车速度
        public double YS { get; set; }//大车速度
        public double ZS { get; set; }//吊具速度
        public DateTime UpdateTime { get; set; }
    }

    public class CraneSpeedLogModel:ObservableObject
    {
        public double XS { get; set; }//小车速度
        public double YS { get; set; }//大车速度
        public double ZS { get; set; }//吊具速度
        public string UpdateTimeWithLog { get; set; }

        private string? _note;
        public string? Note
        {
            get => _note;
            set => SetProperty(ref _note, value);
        }
    }

    public class SplitTimeSpanModel
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public string Display => $"{StartTime:HH:mm:ss} - {EndTime:HH:mm:ss}";
    }

}
