using AvaloniaApplication4.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication4.Service
{
    public class EquipDataService
    {
        private Dictionary<string, bool> _wasCleared = new(); // 清空标志
        private Dictionary<string, DateTime> _lastRecordedSecond = new();
        private Dictionary<string, DateTime> _lastUpdateDate = new();

        public Dictionary<string, List<CraneSpeedModel>> _cranes = new Dictionary<string, List<CraneSpeedModel>>();
        private readonly Random _random = new Random();
        private object _updateLock = new object();
        public EquipDataService()
        {
            _cranes["ECraneA01"] = new List<CraneSpeedModel>();
            _cranes["ECraneA02"] = new List<CraneSpeedModel>();
            _cranes["ECraneA03"] = new List<CraneSpeedModel>();
            _cranes["ECraneW01"] = new List<CraneSpeedModel>();
            _cranes["ECraneW02"] = new List<CraneSpeedModel>();
            _cranes["ECraneW03"] = new List<CraneSpeedModel>();
            _cranes["ECrane01"]=new List<CraneSpeedModel>();
            _cranes["ECrane02"] = new List<CraneSpeedModel>();
            //2025-06-26 15:56:42
            //Generate(new DateTime(2025, 6, 26, 15, 56, 30), new DateTime(2025, 7, 7, 12, 0, 0));
        }

        public void Generate(DateTime startTime, DateTime endTime)
        {
            // 如果结束时间早于开始时间，不做任何操作
            if (endTime < startTime) return;

            // 计算总秒数
            int totalSeconds = (int)(endTime - startTime).TotalSeconds;

            for (int i = 0; i <= totalSeconds; i++)
            {
                // 计算当前时间点
                DateTime currentDateTime = startTime.AddSeconds(i);

                // 创建新的 CraneSpeedModel 实例
                var newModel = new CraneSpeedModel
                {
                    UpdateTime = currentDateTime,
                    XS = _random.Next(0, 51),
                    YS = _random.Next(0, 31),
                    ZS = _random.Next(0, 21)
                };

                // 添加到数据列表
                _cranes["ECraneA01"].Add(newModel);
            }
        }

        public Dictionary<string, List<CraneSpeedModel>> GetCraneSpeedModels()
        {
            return _cranes;
        }

        public bool CheckAndResetClearedFlag(string code)
        {
            if (_wasCleared.TryGetValue(code, out var was) && was)
            {
                _wasCleared[code] = false;
                return true;
            }
            return false;
        }

        public void UpdateCranes(string topic, string content)
        {
            lock (_updateLock)
            {
                if (!topic.StartsWith("ICS/EQ_STATE/")) return;

                string ecraneCode = topic.Split('/')[2];
                var craneMessage = JsonConvert.DeserializeObject<MainData>(content);

                if (!_cranes.ContainsKey(ecraneCode))
                    _cranes[ecraneCode] = new List<CraneSpeedModel>();

                var now  = DateTime.Now;
                var currentSecond = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second); // 精确到秒

               

                 //👉 跳过重复秒的记录
                if (_lastRecordedSecond.TryGetValue(ecraneCode, out var last) && last == currentSecond)
                {
                    return;
                }

                _lastRecordedSecond[ecraneCode] = currentSecond;

                var newModel = new CraneSpeedModel
                {
                    UpdateTime = now,
                    XS = (double)craneMessage.XS,
                    YS = (double)craneMessage.YS,
                    ZS = (double)craneMessage.ZS
                };


                // ⏰ 跨天判断
                DateTime currentDate = now.Date;
                if (_lastUpdateDate.TryGetValue(ecraneCode, out var lastDate) && lastDate < currentDate)
                {
                    if (_cranes[ecraneCode].Count > 0)
                    {
                        SaveCraneDataToFileByDay(ecraneCode, _cranes[ecraneCode], lastDate);
                        _cranes[ecraneCode].Clear();
                    }
                }

                _lastUpdateDate[ecraneCode] = currentDate;

                // 添加新数据
                _cranes[ecraneCode].Add(newModel);

                // 超过 3600 条时，存储前面一小时数据
                if (_cranes[ecraneCode].Count > 120)
                {
                    var saveChunk = _cranes[ecraneCode].Take(120).ToList();
                    SaveCraneDataToFileByDay(ecraneCode, saveChunk, currentDate);
                    _cranes[ecraneCode].RemoveRange(0, 120); 
                    _wasCleared[ecraneCode] = true;
                }
            }
        }


        private void SaveCraneDataToFileByDay(string craneCode, List<CraneSpeedModel> data, DateTime date)
        {
            string datePart = date.ToString("yyyyMMdd");
            string folderPath = Path.Combine(AppContext.BaseDirectory, "SavedCraneData", craneCode);
            Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, $"{craneCode}_{datePart}.json");

            List<CraneSpeedModel> existing = new();
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                existing = JsonConvert.DeserializeObject<List<CraneSpeedModel>>(json) ?? new();
            }

            existing.AddRange(data);

            File.WriteAllText(filePath, JsonConvert.SerializeObject(existing, Formatting.Indented));

            Console.WriteLine($"✅ 已保存 {data.Count} 条到 {filePath}");
        }



        public List<CraneSpeedModel> GetCraneSpeedModelsAccording2Log(string eqCode, DateTime startTime, DateTime endTime)
        {
            if (_cranes.TryGetValue(eqCode, out List<CraneSpeedModel> craneSpeedModels))
            {
                // 筛选出 UpdateDate 在 startTime 前三秒和 endTime 后三秒之间的数据
                return craneSpeedModels.Where(model =>
                    model.UpdateTime >= startTime.AddSeconds(-3) &&
                    model.UpdateTime <= endTime.AddSeconds(3))
                    .ToList();
            }
            else
            {
                // 如果字典中没有对应的键，返回空列表
                return new List<CraneSpeedModel>();
            }
        }
    }
}
