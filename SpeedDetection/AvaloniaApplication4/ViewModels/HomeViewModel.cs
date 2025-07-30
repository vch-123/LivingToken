using AvaloniaApplication4.Models;
using AvaloniaApplication4.Service;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AvaloniaApplication4.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        private bool _xsSelected = true;
        private bool _ysSelected = true;
        private bool _zsSelected = true;

        private const byte OpaqueAlpha = 255;
        private const byte TransparentAlpha = 60;

        private const double PointSizeNormal = 10;
        private const double PointSizeSmall = 2.5;

        public ObservableCollection<ISeries> Series { get; set; }
        public Axis[] XAxes { get; set; }

        // 公开属性，支持绑定按钮 IsChecked
        public bool XSSelected
        {
            get => _xsSelected;
            set
            {
                if (_xsSelected != value)
                {
                    _xsSelected = value;
                    OnPropertyChanged();
                    UpdateOpacity();
                }
            }
        }

        public bool YSSelected
        {
            get => _ysSelected;
            set
            {
                if (_ysSelected != value)
                {
                    _ysSelected = value;
                    OnPropertyChanged();
                    UpdateOpacity();
                }
            }
        }

        public bool ZSSelected
        {
            get => _zsSelected;
            set
            {
                if (_zsSelected != value)
                {
                    _zsSelected = value;
                    OnPropertyChanged();
                    UpdateOpacity();
                }
            }
        }

        public ICommand ToggleXSCommand { get; }
        public ICommand ToggleYSCommand { get; }
        public ICommand ToggleZSCommand { get; }

        private List<CraneSpeedModel> _data;

        public HomeViewModel()
        {
            var random = new Random();
            double min = -1500;
            double max = 1500;
            var now = DateTime.Now.AddMinutes(-700);
            _data = new List<CraneSpeedModel>();

            bool useAbs = App.Services.GetRequiredService<AppSettingsService>().UseAbsoluteValue;

            for (int i = 0; i < 60; i++)
            {
                _data.Add(new CraneSpeedModel
                {
                    UpdateTime = now.AddMinutes(i),
                    XS = Math.Round(random.NextDouble() * (max - min) + min, 2),
                    YS = Math.Round(random.NextDouble() * (max - min) + min, 2),
                    ZS = Math.Round(random.NextDouble() * (max - min) + min, 2),
                });
            }

            Series = new ObservableCollection<ISeries>
    {
        CreateLineSeries("XS小车速度(mm/s)", _data.Select(d => new DateTimePoint(d.UpdateTime, useAbs ? Math.Abs(d.XS) : d.XS)), SKColors.DodgerBlue, XSSelected),
        CreateLineSeries("YS大车速度(mm/s)", _data.Select(d => new DateTimePoint(d.UpdateTime, useAbs ? Math.Abs(d.YS) : d.YS)), SKColors.OrangeRed, YSSelected),
        CreateLineSeries("ZS吊具速度(mm/s)", _data.Select(d => new DateTimePoint(d.UpdateTime, useAbs ? Math.Abs(d.ZS) : d.ZS)), SKColors.MediumSeaGreen, ZSSelected)
    };

            XAxes = new Axis[]
            {
                new Axis
                {
                    Labeler = value => new DateTime((long)value).ToString("HH:mm"),
                    UnitWidth = TimeSpan.FromMinutes(1).Ticks,
                    MinLimit = _data.First().UpdateTime.Ticks,
                    MaxLimit = _data.Last().UpdateTime.Ticks
                }
            };

            ToggleXSCommand = new RelayCommand(() => XSSelected = !XSSelected);
            ToggleYSCommand = new RelayCommand(() => YSSelected = !YSSelected);
            ToggleZSCommand = new RelayCommand(() => ZSSelected = !ZSSelected);

            UpdateOpacity();
        }

        private void UpdateOpacity()
        {
            SetSeriesOpacity(0, XSSelected, SKColors.DodgerBlue);
            SetSeriesOpacity(1, YSSelected, SKColors.OrangeRed);
            SetSeriesOpacity(2, ZSSelected, SKColors.MediumSeaGreen);
            OnPropertyChanged(nameof(Series));
        }

        private void SetSeriesOpacity(int index, bool isSelected, SKColor baseColor)
        {
            if (Series[index] is LineSeries<DateTimePoint> lineSeries)
            {
                var alpha = isSelected ? OpaqueAlpha : TransparentAlpha;
                lineSeries.Stroke = new SolidColorPaint(new SKColor(baseColor.Red, baseColor.Green, baseColor.Blue, alpha), 2);
                lineSeries.Fill = isSelected
                    ? new SolidColorPaint(new SKColor(baseColor.Red, baseColor.Green, baseColor.Blue, 140))
                    : null;  // 未选中时不填充
                lineSeries.GeometrySize = isSelected ? PointSizeNormal : PointSizeSmall;
            }
        }

        private LineSeries<DateTimePoint> CreateLineSeries(string name, IEnumerable<DateTimePoint> points, SKColor color, bool selected)
        {
            var alpha = selected ? OpaqueAlpha : TransparentAlpha;
            return new LineSeries<DateTimePoint>
            {
                Name = name,
                Values = points.ToArray(),
                Stroke = new SolidColorPaint(new SKColor(color.Red, color.Green, color.Blue, alpha), 2),
                Fill = selected
                    ? new SolidColorPaint(new SKColor(color.Red, color.Green, color.Blue, 140))
                    : null,
                LineSmoothness = 0,
                GeometrySize = selected ? PointSizeNormal : PointSizeSmall,
            };
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        public RelayCommand(Action execute) => _execute = execute;
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => _execute();
        public event EventHandler? CanExecuteChanged { add { } remove { } }
    }
}
