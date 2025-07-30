using AvaloniaApplication4.Models;
using AvaloniaApplication4.Service;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.Drawing.Segments;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia.Dto;

namespace AvaloniaApplication4.ViewModels
{
    public class LogThreeViewModel : INotifyPropertyChanged
    {

        private string _selectedFileName = "未选择文件";
        public string SelectedFileName
        {
            get => _selectedFileName;
            set { _selectedFileName = value; OnPropertyChanged(); }
        }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        private string _selectedTaskLogFileName = "未选择文件";
        public string SelectedTaskLogFileName
        {
            get => _selectedTaskLogFileName;
            set { _selectedTaskLogFileName = value; OnPropertyChanged(); }
        }

        private string _taskLogStatusMessage = string.Empty;
        public string TaskLogStatusMessage
        {
            get => _taskLogStatusMessage;
            set { _taskLogStatusMessage = value; OnPropertyChanged(); }
        }

        private bool _xsSelected = true;
        private bool _ysSelected = true;
        private bool _zsSelected = true;

        private const byte OpaqueAlpha = 255;
        private const byte TransparentAlpha = 60;
        private const double PointSizeNormal = 10;
        private const double PointSizeSmall = 2.5;

        public ObservableCollection<LogModel> LogModels { get; set; } = new ObservableCollection<LogModel>();
        public ObservableCollection<SplitTimeSpanModel> SplitTimeSpanModels { get; set; } = new ObservableCollection<SplitTimeSpanModel>();
        public ObservableCollection<ISeries> Series { get; set; }
        public ObservableCollection<ISeries> XSeries { get; set; }
        public ObservableCollection<ISeries> YSeries { get; set; }
        public ObservableCollection<ISeries> ZSeries { get; set; }

        public Axis[] XAxes { get; set; }
        public Axis[] YAxes { get; set; }

        public ICommand LocateCommand { get; }

        private LogModel? _selectedLogModel;
        public LogModel? SelectedLogModel
        {
            get => _selectedLogModel;
            set
            {
                if (_selectedLogModel != value)
                {
                    _selectedLogModel = value;
                    OnPropertyChanged();
                }
            }
        }

        private SplitTimeSpanModel? _selectedTimeSpanModel;
        public SplitTimeSpanModel? SelectedTimeSpanModel
        {
            get => _selectedTimeSpanModel;
            set
            {
                if (_selectedTimeSpanModel != value)
                {
                    _selectedTimeSpanModel = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand DrawCommand { get; }


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


        public List<string> Labels = new List<string>();
        private List<CraneSpeedModel> _craneSpeedModels;
        public List<CraneSpeedLogModel> _craneSpeedLogModels;

        public List<CraneSpeedLogModel> _craneSpeedLogModelsOnChart;
        public List<CraneSpeedModel> _craneSpeedModelsTask;
        public List<CraneSpeedLogModel> _craneSpeedLogModelsTask;
        private EquipDataService _equipDataService;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        public LogThreeViewModel()
        {
            DrawCommand = new RelayCommand(DrawSegment);
            LocateCommand = new RelayCommand(LocateSelectedLog);

            XSeries = new ObservableCollection<ISeries>
    {
        new LineSeries<double>
        {
            Name = "XS小车速度",
            Values = new ObservableCollection<double>(),
            Stroke = new SolidColorPaint(SKColors.DodgerBlue, 2),
            Fill = new SolidColorPaint(SKColors.DodgerBlue.WithAlpha(50)),
            GeometrySize = PointSizeNormal,
            LineSmoothness = 0.5
        }
    };

            YSeries = new ObservableCollection<ISeries>
    {
        new LineSeries<double>
        {
            Name = "YS大车速度",
            Values = new ObservableCollection<double>(),
            Stroke = new SolidColorPaint(SKColors.OrangeRed, 2),
            Fill = new SolidColorPaint(SKColors.OrangeRed.WithAlpha(50)),
            GeometrySize = PointSizeNormal,
            LineSmoothness = 0.5
        }
    };

            ZSeries = new ObservableCollection<ISeries>
    {
        new LineSeries<double>
        {
            Name = "ZS吊具速度",
            Values = new ObservableCollection<double>(),
            Stroke = new SolidColorPaint(SKColors.MediumSeaGreen, 2),
            Fill = new SolidColorPaint(SKColors.MediumSeaGreen.WithAlpha(50)),
            GeometrySize = PointSizeNormal,
            LineSmoothness = 0.5
        }
    };

        }

        private bool useAbs => App.Services.GetRequiredService<AppSettingsService>().UseAbsoluteValue;

        private void DrawSegment()
        {
            SplitTimeSpanModel segment = SelectedTimeSpanModel;


            if (_craneSpeedLogModels == null || _craneSpeedLogModels.Count == 0)
                return;

            TimeSpan start = segment.StartTime.TimeOfDay;
            TimeSpan end = segment.EndTime.TimeOfDay;

            // 提前转为 TimeSpan 列表（所有 UpdatetimeWithLog 都是 "HH:mm:ss" 格式）
            List<TimeSpan> timeList = _craneSpeedLogModels
                .Select(m =>
                {
                    var timeStr = m.UpdateTimeWithLog;
                    if (timeStr.Contains(' '))
                        timeStr = timeStr.Split(' ')[1]; // 取出 "HH:mm:ss"
                    return TimeSpan.Parse(timeStr);
                })
                .ToList();


            // 找起点
            int startIndex = timeList.BinarySearch(start);
            if (startIndex < 0) startIndex = ~startIndex;

            // 找终点
            int endIndex = timeList.BinarySearch(end);
            if (endIndex < 0) endIndex = ~endIndex;

            // 截取区间（左闭右开）
            var filtered = _craneSpeedLogModels.Skip(startIndex).Take(endIndex - startIndex).ToList();
            _craneSpeedLogModelsOnChart = filtered;
            // 👉 这里你可以拿 filtered 去绘图
            Draw();
        }



        private void Draw()
        {
            var simulatedData = _craneSpeedLogModelsOnChart;
            var xValues = simulatedData.Select(p => p.UpdateTimeWithLog).ToList();

            var yValuesXS = useAbs ? simulatedData.Select(p => Math.Abs(p.XS)).ToList() : simulatedData.Select(p => p.XS).ToList();
            var yValuesYS = useAbs ? simulatedData.Select(p => Math.Abs(p.YS)).ToList() : simulatedData.Select(p => p.YS).ToList();
            var yValuesZS = useAbs ? simulatedData.Select(p => Math.Abs(p.ZS)).ToList() : simulatedData.Select(p => p.ZS).ToList();

            ((LineSeries<double>)XSeries[0]).Values = yValuesXS;
            ((LineSeries<double>)YSeries[0]).Values = yValuesYS;
            ((LineSeries<double>)ZSeries[0]).Values = yValuesZS;

            XAxes = new Axis[]
            {
        new Axis
        {
            Labeler = value =>
            {
                int index = (int)value;
                if (index >= 0 && index < xValues.Count)
                    return xValues[index];
                return "";
            },
            UnitWidth = 1,
            MinLimit = 0,
            MaxLimit = xValues.Count - 1
        }
            };

            YAxes = new Axis[]
            {
        new Axis { Labeler = value => value.ToString("0") }
            };

            OnPropertyChanged(nameof(XSeries));
            OnPropertyChanged(nameof(YSeries));
            OnPropertyChanged(nameof(ZSeries));
            OnPropertyChanged(nameof(XAxes));
            OnPropertyChanged(nameof(YAxes));
        }


        public void LoadLogFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    StatusMessage = "文件不存在";
                    return;
                }

                var json = File.ReadAllText(filePath);

                var rawModels = JsonConvert.DeserializeObject<List<CraneSpeedModel>>(json);

                if (rawModels == null)
                {
                    StatusMessage = "文件内容解析失败";
                    return;
                }

                var logModels = rawModels.Select(model => new CraneSpeedLogModel
                {
                    XS = model.XS,
                    YS = model.YS,
                    ZS = model.ZS,
                    UpdateTimeWithLog = model.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
                }).ToList();

                _craneSpeedLogModels = new List<CraneSpeedLogModel>();
                _craneSpeedLogModels = logModels;
                SelectedFileName = Path.GetFileName(filePath);
                StatusMessage = $"✅ 成功读取 {_craneSpeedLogModels.Count} 条数据";
                SplitTimeSpan(rawModels);
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ 读取失败: {ex.Message}";
            }
        }

        private void SplitTimeSpan(List<CraneSpeedModel> rawModels)
        {
            try
            {

                // 1. 创建日志模型集合
                var logModels = rawModels.Select(m => new CraneSpeedLogModel
                {
                    XS = m.XS,
                    YS = m.YS,
                    ZS = m.ZS,
                    UpdateTimeWithLog = m.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                }).ToList();


                // 2. 分段
                SplitTimeSpanModels.Clear();

                var minTime = rawModels.Min(m => m.UpdateTime);
                var maxTime = rawModels.Max(m => m.UpdateTime);
                var current = minTime;

                while (current < maxTime)
                {
                    var next = current.AddMinutes(30);
                    SplitTimeSpanModels.Add(new SplitTimeSpanModel
                    {
                        StartTime = current,
                        EndTime = next
                    });

                    current = next;
                }

                StatusMessage = $"✅ 成功读取 {logModels.Count} 条数据，已分段 {SplitTimeSpanModels.Count} 段";

                // 自动选择第一个时间段
                if (SplitTimeSpanModels.Count > 0)
                {
                    SelectedTimeSpanModel = SplitTimeSpanModels[0];
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"❌ 读取失败: {ex.Message}";
            }
        }

        private void UpdateOpacity()
        {
            SetSeriesOpacity(XSeries, XSSelected, SKColors.DodgerBlue);
            SetSeriesOpacity(YSeries, YSSelected, SKColors.OrangeRed);
            SetSeriesOpacity(ZSeries, ZSSelected, SKColors.MediumSeaGreen);

            OnPropertyChanged(nameof(XSeries));
            OnPropertyChanged(nameof(YSeries));
            OnPropertyChanged(nameof(ZSeries));
        }

        private void SetSeriesOpacity(ObservableCollection<ISeries> seriesCollection, bool isSelected, SKColor baseColor)
        {
            if (_craneSpeedLogModels.Count == 0) return;

            if (seriesCollection[0] is LineSeries<double> lineSeries)
            {
                var alpha = isSelected ? OpaqueAlpha : TransparentAlpha;
                lineSeries.Stroke = new SolidColorPaint(new SKColor(baseColor.Red, baseColor.Green, baseColor.Blue, alpha), 2);
                lineSeries.Fill = isSelected
                    ? new SolidColorPaint(new SKColor(baseColor.Red, baseColor.Green, baseColor.Blue, 140))
                    : null;
                lineSeries.GeometrySize = isSelected ? PointSizeNormal : PointSizeSmall;
            }
        }

        public void LoadTaskLogFile(string taskFilePath)
        {
            if (_craneSpeedLogModels is null || _craneSpeedLogModels.Count == 0) { TaskLogStatusMessage = "请先加载数据文件"; return; }
            try
            {
                ObservableCollection<LogModel> logModels = new();
                string? currentInfo = null;
                DateTime logTime;

                using StreamReader reader = new(taskFilePath);
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("info:"))
                    {
                        int timeEndIndex = line.IndexOf(" 星期");
                        if (timeEndIndex != -1)
                        {
                            string timeStr = line.Substring("info: ".Length, timeEndIndex - "info: ".Length);
                            if (DateTime.TryParse(timeStr, out logTime))
                            {
                                currentInfo = reader.ReadLine()?.Trim();
                                if (!string.IsNullOrWhiteSpace(currentInfo))
                                {
                                    logModels.Add(new LogModel { LogTime = logTime, Info = currentInfo });
                                }
                            }
                        }
                    }
                }

                LogModels = logModels;
                OnPropertyChanged(nameof(LogModels));
                _craneSpeedModelsTask = GetTaskLog1(_craneSpeedLogModels, logModels.First().LogTime.AddSeconds(-3), logModels.Last().LogTime.AddSeconds(3));

                var simulatedData = _craneSpeedModelsTask;
                List<string> Labels = GetLabelsWithLogInfo(simulatedData.Select(x => x.UpdateTime.ToString("HH:mm:ss")).ToList());

                _craneSpeedLogModelsTask = new();
                for (int i = 0; i < Labels.Count; i++)
                {
                    _craneSpeedLogModelsTask.Add(new CraneSpeedLogModel()
                    {
                        XS = _craneSpeedModelsTask[i].XS,
                        YS = _craneSpeedModelsTask[i].YS,
                        ZS = _craneSpeedModelsTask[i].ZS,
                        UpdateTimeWithLog = Labels[i] // 保留时间字符串
                    });
                }
                if (LogModels.Count > 0)
                {
                    SelectedLogModel = LogModels.First();
                }


                SelectedTaskLogFileName = Path.GetFileName(taskFilePath);
                TaskLogStatusMessage = $"成功读取 {logModels.Count} 条日志记录";

            }
            catch (Exception ex)
            {
                StatusMessage = $"读取失败: {ex.Message}";
            }
        }

        private List<string> GetLabelsWithLogInfo(List<string> list)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            foreach (var item in LogModels)
            {
                keyValuePairs[item.LogTime.ToString("HH:mm:ss")] = item.Info;
            }

            List<string> resultLabels = new List<string>();
            foreach (var time in list)
            {
                if (keyValuePairs.ContainsKey(time))
                {
                    resultLabels.Add($"{time} - {keyValuePairs[time]}");
                }
                else
                {
                    resultLabels.Add(time);
                }
            }

            return resultLabels;
        }

        private List<CraneSpeedModel> GetTaskLog(List<CraneSpeedLogModel> craneSpeedLogModels, DateTime logTime1, DateTime logTime2)
        {
            var result = new List<CraneSpeedModel>();

            foreach (var log in craneSpeedLogModels)
            {
                if (DateTime.TryParseExact(log.UpdateTimeWithLog, "HH:mm:ss",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out DateTime parsedTime))
                {
                    // 如果解析成功，且时间在区间范围内，则添加
                    if (parsedTime >= logTime1 && parsedTime <= logTime2)
                    {
                        result.Add(new CraneSpeedModel
                        {
                            XS = log.XS,
                            YS = log.YS,
                            ZS = log.ZS,
                            UpdateTime = parsedTime
                        });
                    }
                }
            }

            return result;
        }


        private List<CraneSpeedModel> GetTaskLog1(List<CraneSpeedLogModel> craneSpeedLogModels, DateTime logTime1, DateTime logTime2)
        {
            var result = new List<CraneSpeedModel>();

            var timeStart = logTime1.TimeOfDay;
            var timeEnd = logTime2.TimeOfDay;

            foreach (var log in craneSpeedLogModels)
            {
                string timeStr = log.UpdateTimeWithLog;

                // 支持有日期前缀的情况，提取 HH:mm:ss
                if (timeStr.Contains(' '))
                {
                    var parts = timeStr.Split(' ');
                    timeStr = parts.Length > 1 ? parts[1] : parts[0];
                }

                if (TimeSpan.TryParseExact(timeStr, "hh\\:mm\\:ss", null, out TimeSpan parsedTime))
                {
                    if (parsedTime >= timeStart && parsedTime <= timeEnd)
                    {
                        result.Add(new CraneSpeedModel
                        {
                            XS = log.XS,
                            YS = log.YS,
                            ZS = log.ZS,
                            UpdateTime = DateTime.Today.Add(parsedTime) // 构造 DateTime 可用作显示或定位
                        });
                    }
                }
            }

            return result;
        }


        public void DrawTaskChart()
        {
            if (_craneSpeedLogModelsTask == null || _craneSpeedLogModelsTask.Count == 0) return;

            var xValues = _craneSpeedLogModelsTask.Select(p => p.UpdateTimeWithLog).ToList();

            var yValuesXS = useAbs ? _craneSpeedLogModelsTask.Select(p => Math.Abs(p.XS)).ToList() : _craneSpeedLogModelsTask.Select(p => p.XS).ToList();
            var yValuesYS = useAbs ? _craneSpeedLogModelsTask.Select(p => Math.Abs(p.YS)).ToList() : _craneSpeedLogModelsTask.Select(p => p.YS).ToList();
            var yValuesZS = useAbs ? _craneSpeedLogModelsTask.Select(p => Math.Abs(p.ZS)).ToList() : _craneSpeedLogModelsTask.Select(p => p.ZS).ToList();

            ((LineSeries<double>)XSeries[0]).Values = yValuesXS;
            ((LineSeries<double>)YSeries[0]).Values = yValuesYS;
            ((LineSeries<double>)ZSeries[0]).Values = yValuesZS;

            XAxes = new Axis[]
            {
        new Axis
        {
            Labeler = value =>
            {
                int index = (int)value;
                if (index >= 0 && index < xValues.Count)
                    return xValues[index];
                return "";
            },
            UnitWidth = 1,
            MinLimit = 0,
            MaxLimit = xValues.Count - 1
        }
            };

            YAxes = new Axis[]
            {
        new Axis { Labeler = value => value.ToString("0") }
            };

            OnPropertyChanged(nameof(XAxes));
            OnPropertyChanged(nameof(YAxes));
            OnPropertyChanged(nameof(XSeries));
            OnPropertyChanged(nameof(YSeries));
            OnPropertyChanged(nameof(ZSeries));
        }


        private void LocateSelectedLog()
        {
            if (SelectedLogModel == null || _craneSpeedLogModelsTask == null || _craneSpeedLogModelsTask.Count == 0)
                return;

            var targetTime = SelectedLogModel.LogTime.ToString("HH:mm:ss");
            int targetIndex = _craneSpeedLogModelsTask.FindIndex(log =>
                log.UpdateTimeWithLog.StartsWith(targetTime)); // 支持 "HH:mm:ss - 注释" 形式

            if (targetIndex < 0) return;

            // 更新 XAxis 的显示区间，使目标居中
            int windowSize = 9;
            int half = windowSize / 2;
            int min = Math.Max(0, targetIndex - half);
            int max = Math.Min(_craneSpeedLogModelsTask.Count - 1, targetIndex + half);

            if (XAxes != null && XAxes.Length > 0)
            {
                XAxes[0].MinLimit = min;
                XAxes[0].MaxLimit = max;
                OnPropertyChanged(nameof(XAxes));
            }

        }

    }
}