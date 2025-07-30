using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Timers;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using AvaloniaApplication4.Service;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace AvaloniaApplication4.ViewModels
{
    public partial class TestViewModel : ObservableObject
    {
        private readonly Timer _timer;
        private const int _maxPoints = 600; // 最多保留10分钟数据（假设每秒1条）
        private const int _viewWindow = 14; // 视图显示最近30条

        private ObservableCollection<CraneSpeedLogModel> _dataPoints = new();

        private readonly EquipDataService _equipDataService;


        public ObservableCollection<ISeries> Series { get; set; }

        public Axis[] XAxes { get; set; }
        public Axis[] YAxes { get; set; }

        private int _currentIndex = 0;

        private bool _autoScroll = true;
        public bool AutoScroll
        {
            get => _autoScroll;
            set
            {
                if (_autoScroll != value)
                {
                    _autoScroll = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _xsSelected = true;
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

        private bool _ysSelected = true;
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

        private bool _zsSelected = true;
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

        private int? _hoverIndex = null;
        private int? _selectedIndex = null;

        public int? HoverIndex
        {
            get => _hoverIndex;
            private set
            {
                if (_hoverIndex != value)
                {
                    _hoverIndex = value;
                    OnPropertyChanged();
                    // 可以触发刷新高亮显示
                }
            }
        }

        public int? SelectedIndex
        {
            get => _selectedIndex;
            private set
            {
                if (_selectedIndex != value)
                {
                    _selectedIndex = value;
                    OnPropertyChanged();
                    // 点击选中点，处理业务逻辑
                }
            }
        }


        private CraneSpeedLogModel? _selectedPoint;
        public CraneSpeedLogModel? SelectedPoint
        {
            get => _selectedPoint;
            set
            {
                SetProperty(ref _selectedPoint, value);
                // 可以触发高亮显示、详细信息弹窗等
            }
        }


        public void UpdateHoverIndexFromPosition(double pixelX, double chartWidth)
        {
            int count = _dataPoints.Count;
            if (count == 0) return;

            int visibleCount = _viewWindow;
            int windowStartIndex = Math.Max(0, count - visibleCount);

            double ratio = pixelX / chartWidth;
            int relativeIndex = (int)(ratio * visibleCount);
            relativeIndex = Math.Clamp(relativeIndex, 0, visibleCount - 1);

            int absoluteIndex = windowStartIndex + relativeIndex;

            HoverIndex = absoluteIndex;
        }


        public void SelectPointFromPosition(double pixelX, double chartWidth)
        {
            if (_dataPoints.Count == 0 || chartWidth == 0) return;

            // 获取当前可视范围
            var axis = XAxes[0];
            double minLimit = axis.MinLimit ?? 0;
            double maxLimit = axis.MaxLimit ?? _dataPoints.Count - 1;

            double visibleCount = maxLimit - minLimit;
            double ratio = pixelX / chartWidth;

            // 对应真实数据下标
            double realIndex = minLimit + ratio * visibleCount;
            int nearestIndex = (int)Math.Round(realIndex);
            nearestIndex = Math.Clamp(nearestIndex, 0, _dataPoints.Count - 1);

            SelectedPoint = _dataPoints[nearestIndex];

            Console.WriteLine($"🎯选中点 Index={nearestIndex} => {SelectedPoint.UpdateTimeWithLog} XS={SelectedPoint.XS:F2} YS={SelectedPoint.YS:F2} ZS={SelectedPoint.ZS:F2}");
        }

        [ObservableProperty]
        private string? newNoteText;

        // 之前的SelectedPoint等成员保持不变...

        // 更新备注的普通方法，稍后在Code-behind调用

        public void UpdateNoteWithText(string note)
        {
            if (SelectedPoint != null)
            {
                SelectedPoint.Note = note;

                // 只保留时间部分，附加备注
                SelectedPoint.UpdateTimeWithLog = SelectedPoint.UpdateTimeWithLog.Split(' ')[0] + $" {note}";

                Console.WriteLine($"📝 备注已更新: {SelectedPoint.Note}");

                // 通知属性变更
                OnPropertyChanged(nameof(SelectedPoint));

                // 居中显示该点，最多显示8个
                int idx = _dataPoints.IndexOf(SelectedPoint);
                if (idx >= 0)
                {
                    const int windowSize = 8;
                    int total = _dataPoints.Count;

                    int displayCount = Math.Min(windowSize, total);
                    int minLimit = idx - displayCount / 2;
                    if (minLimit < 0) minLimit = 0;

                    int maxLimit = minLimit + displayCount - 1;
                    if (maxLimit >= total)
                    {
                        maxLimit = total - 1;
                        minLimit = Math.Max(0, maxLimit - displayCount + 1);
                    }

                    if (XAxes != null && XAxes.Length > 0)
                    {
                        XAxes[0].MinLimit = minLimit;
                        XAxes[0].MaxLimit = maxLimit;
                    }

                    OnPropertyChanged(nameof(XAxes));
                }
            }
        }


        private const byte OpaqueAlpha = 255;
        private const byte TransparentAlpha = 60;
        private const double PointSizeNormal = 10;
        private const double PointSizeSmall = 2.5;

        private LineSeries<double> _xsSeries;
        private LineSeries<double> _ysSeries;
        private LineSeries<double> _zsSeries;



        public TestViewModel()
        {
            _equipDataService = App.Services.GetRequiredService<EquipDataService>();


            var blue = SKColor.Parse("#1E88E5");    // XS - 小车速度
            var red = SKColor.Parse("#D32F2F");     // YS - 大车速度
            var green = SKColor.Parse("#43A047");   // ZS - 吊具速度

            _xsSeries = new LineSeries<double>
            {
                Name = "XS小车速度",
                Values = new ObservableCollection<double>(),
                Stroke = new SolidColorPaint(blue, 3), // 增粗
                Fill = new SolidColorPaint(blue.WithAlpha(99)), // 稍高透明度
                GeometrySize = 10, // 显示小点
                GeometryStroke = new SolidColorPaint(blue, 2), // 点边缘
                LineSmoothness = 0.4
            };

            _ysSeries = new LineSeries<double>
            {
                Name = "YS大车速度",
                Values = new ObservableCollection<double>(),
                Stroke = new SolidColorPaint(red, 3),
                Fill = new SolidColorPaint(red.WithAlpha(99)),
                GeometrySize = 10,
                GeometryStroke = new SolidColorPaint(red, 2),
                LineSmoothness = 0.4
            };

            _zsSeries = new LineSeries<double>
            {
                Name = "ZS吊具速度",
                Values = new ObservableCollection<double>(),
                Stroke = new SolidColorPaint(green, 3),
                Fill = new SolidColorPaint(green.WithAlpha(99)),
                GeometrySize = 10,
                GeometryStroke = new SolidColorPaint(green, 2),
                LineSmoothness = 0.4
            };



            //_xsSeries = new LineSeries<double>
            //{
            //    Name = "XS小车速度",
            //    Values = new ObservableCollection<double>(),
            //    Stroke = new SolidColorPaint(SKColors.DodgerBlue, 2),
            //    Fill = new SolidColorPaint(SKColors.DodgerBlue.WithAlpha(50)),
            //    GeometrySize = PointSizeNormal,
            //    LineSmoothness = 0.5
            //};
            //_ysSeries = new LineSeries<double>
            //{
            //    Name = "YS大车速度",
            //    Values = new ObservableCollection<double>(),
            //    Stroke = new SolidColorPaint(SKColors.OrangeRed, 2),
            //    Fill = new SolidColorPaint(SKColors.OrangeRed.WithAlpha(50)),
            //    GeometrySize = PointSizeNormal,
            //    LineSmoothness = 0.5
            //};
            //_zsSeries = new LineSeries<double>
            //{
            //    Name = "ZS吊具速度",
            //    Values = new ObservableCollection<double>(),
            //    Stroke = new SolidColorPaint(SKColors.MediumSeaGreen, 2),
            //    Fill = new SolidColorPaint(SKColors.MediumSeaGreen.WithAlpha(50)),
            //    GeometrySize = PointSizeNormal,
            //    LineSmoothness = 0.5
            //};

            Series = new ObservableCollection<ISeries> { _xsSeries, _ysSeries, _zsSeries };

            XAxes = new Axis[]
            {
                new Axis
                {
                    Labeler = value =>
                    {
                        int index = (int)value;
                        if (index >= 0 && index < _dataPoints.Count)
                            return _dataPoints[index].UpdateTimeWithLog;
                        return string.Empty;
                    },
                    UnitWidth = 1,
                    MinLimit = 0,
                    MaxLimit = _viewWindow
                }
            };

            YAxes = new Axis[]
            {
                new Axis
                {

                    Labeler = val => val.ToString("0.00") // ✅显示如 12.34
                }
            };

            _timer = new Timer(1000);
            _timer.Elapsed += (s, e) => AddRealData();
            _timer.Start();

            UpdateOpacity();
        }
        private DateTime? _lastUpdateTime = null;
        private int _lastSourceCount = 0;


        private List<string> _availableCraneCodes = App.Services.GetRequiredService<AppSettingsService>().Topics.Select(t=>t.Split('/')[2]).ToList();
        public List<string> AvailableCraneCodes
        {
            get => _availableCraneCodes;
            set
            {
                if (_availableCraneCodes != value)
                {
                    _availableCraneCodes = value;
                    OnPropertyChanged(nameof(AvailableCraneCodes));
                }
            }
        }

        private string _selectedCraneCode = "ECrane01";
        public string SelectedCraneCode
        {
            get => _selectedCraneCode;
            set
            {
                if (_selectedCraneCode != value)
                {
                    _selectedCraneCode = value;
                    OnPropertyChanged(nameof(SelectedCraneCode));
                    ClearAllChartData();  // 当切换 crane 时清空图表
                    _lastUpdateTime = null;
                }
            }
        }
        private void OnSelectedCraneCodeChanged(string value)
        {
            ClearAllChartData();    // 清空图表
            _lastUpdateTime = null; // 重置更新时间戳
        }





        private void AddRealData()
        {
            var list = _equipDataService.GetCraneSpeedModels().GetValueOrDefault(SelectedCraneCode);
            if (list == null || list.Count == 0) return;

            if (_equipDataService.CheckAndResetClearedFlag(SelectedCraneCode))
            {
                Dispatcher.UIThread.Post(ClearAllChartData);
                _lastUpdateTime = null;
            }

            // 提前拷贝副本，防止 UI 线程遍历时被修改
            var rawNewPoints = _lastUpdateTime == null
                ? list
                : list.Where(p => p.UpdateTime > _lastUpdateTime);

            var safeCopy = rawNewPoints.ToList(); // ✅ 拷贝副本，确保线程安全

            if (safeCopy.Count == 0) return;

            _lastUpdateTime = safeCopy.Last().UpdateTime;

            bool useAbs = App.Services.GetRequiredService<AppSettingsService>().UseAbsoluteValue;

            Dispatcher.UIThread.Post(() =>
            {
                foreach (var latest in safeCopy)
                {
                    var newData = new CraneSpeedLogModel
                    {
                        XS = (useAbs)?Math.Abs(latest.XS):latest.XS,
                        YS = (useAbs) ? Math.Abs(latest.YS) : latest.YS,
                        ZS = (useAbs) ? Math.Abs(latest.ZS) : latest.ZS,
                        UpdateTimeWithLog = latest.UpdateTime.ToString("HH:mm:ss:fff")
                    };

                    _dataPoints.Add(newData);
                    _currentIndex++;

                    (_xsSeries.Values as ObservableCollection<double>)?.Add(newData.XS);
                    (_ysSeries.Values as ObservableCollection<double>)?.Add(newData.YS);
                    (_zsSeries.Values as ObservableCollection<double>)?.Add(newData.ZS);
                }

                while (_dataPoints.Count > _maxPoints)
                {
                    _dataPoints.RemoveAt(0);
                    (_xsSeries.Values as ObservableCollection<double>)?.RemoveAt(0);
                    (_ysSeries.Values as ObservableCollection<double>)?.RemoveAt(0);
                    (_zsSeries.Values as ObservableCollection<double>)?.RemoveAt(0);
                    _currentIndex--;
                }

                if (AutoScroll) UpdateAxisLimits();

                OnPropertyChanged(nameof(Series));
            });
        }




        private void ClearAllChartData()
        {
            _dataPoints.Clear();
            _currentIndex = 0;
            _lastUpdateTime = null;

            (_xsSeries.Values as ObservableCollection<double>)?.Clear();
            (_ysSeries.Values as ObservableCollection<double>)?.Clear();
            (_zsSeries.Values as ObservableCollection<double>)?.Clear();

            XAxes[0].MinLimit = 0;
            XAxes[0].MaxLimit = _viewWindow;

            OnPropertyChanged(nameof(Series));
            OnPropertyChanged(nameof(XAxes));
        }


        private void AddRandomData()
        {
            var now = DateTime.Now;

            var newData = new CraneSpeedLogModel
            {
                XS = Random.Shared.NextDouble() * 40,
                YS = Random.Shared.NextDouble() * 40,
                ZS = Random.Shared.NextDouble() * 40,
                UpdateTimeWithLog = now.ToString("HH:mm:ss")
            };

            Dispatcher.UIThread.Post(() =>
            {
                _dataPoints.Add(newData);
                _currentIndex++;

                if (_dataPoints.Count > _maxPoints)
                {
                    _dataPoints.RemoveAt(0);
                    _currentIndex--;
                }

                // 添加新数据
                (_xsSeries.Values as ObservableCollection<double>)?.Add(newData.XS);
                (_ysSeries.Values as ObservableCollection<double>)?.Add(newData.YS);
                (_zsSeries.Values as ObservableCollection<double>)?.Add(newData.ZS);

                if ((_xsSeries.Values as ObservableCollection<double>)!.Count > _maxPoints)
                {
                    (_xsSeries.Values as ObservableCollection<double>)!.RemoveAt(0);
                    (_ysSeries.Values as ObservableCollection<double>)!.RemoveAt(0);
                    (_zsSeries.Values as ObservableCollection<double>)!.RemoveAt(0);
                }

                // 只有自动跟随时更新坐标轴窗口，否则保持固定
                if (AutoScroll)
                {
                    UpdateAxisLimits();
                    SelectedIndex = null;  // 自动跟随时，清空选中点，避免冲突
                }
                else
                {
                    // 关闭自动跟随，选中点不变
                }

                OnPropertyChanged(nameof(Series));
            });
        }

        private void UpdateAxisLimits()
        {
            var count = _dataPoints.Count;
            if (XAxes[0] is Axis axis)
            {
                axis.MinLimit = Math.Max(0, count - _viewWindow);
                axis.MaxLimit = count - 1;
            }
            OnPropertyChanged(nameof(XAxes));
        }

        private void UpdateOpacity()
        {
            SetSeriesOpacity(_xsSeries, XSSelected, SKColors.DodgerBlue);
            SetSeriesOpacity(_ysSeries, YSSelected, SKColors.OrangeRed);
            SetSeriesOpacity(_zsSeries, ZSSelected, SKColors.MediumSeaGreen);
            OnPropertyChanged(nameof(Series));
        }

        private void SetSeriesOpacity(LineSeries<double> series, bool isSelected, SKColor baseColor)
        {
            var alpha = isSelected ? OpaqueAlpha : TransparentAlpha;
            series.Stroke = new SolidColorPaint(new SKColor(baseColor.Red, baseColor.Green, baseColor.Blue, alpha), 2);
            series.Fill = isSelected
                ? new SolidColorPaint(new SKColor(baseColor.Red, baseColor.Green, baseColor.Blue, 50))
                : null;
            series.GeometrySize = isSelected ? PointSizeNormal : PointSizeSmall;
        }
    }

    public class CraneSpeedLogModel : ObservableObject
    {
        public double XS { get; set; }
        public double YS { get; set; }
        public double ZS { get; set; }

        public string UpdateTimeWithLog { get; set; } = string.Empty;

        private string? _note;
        public string? Note
        {
            get => _note;
            set => SetProperty(ref _note, value);
        }
    }

}
