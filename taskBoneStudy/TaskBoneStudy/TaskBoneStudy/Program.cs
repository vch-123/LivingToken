using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public interface ITaskCommand<T> where T : ITaskModel
{
    Task<ITaskCommand<T>> ExecuteAsync(T model, CancellationToken token);
}

public interface ITaskModel
{
    string Code { get; set; }
}

public class CraneTaskModel : ITaskModel
{
    public string Code { get; set; }
    public ITaskCommand<CraneTaskModel> TaskCommand { get; set; }
    public int StepCounter { get; set; } = 0;
}

// === 命令流 ===

public class CraneVerifyOrder : ITaskCommand<CraneTaskModel>
{
    public Task<ITaskCommand<CraneTaskModel>> ExecuteAsync(CraneTaskModel model, CancellationToken token)
    {
        Console.WriteLine();
        Console.WriteLine($"[{model.Code}] ✔ 验证任务 - 第{model.StepCounter++}步");
        return Task.FromResult<ITaskCommand<CraneTaskModel>>(new CraneWalk());
    }
}

public class CraneWalk : ITaskCommand<CraneTaskModel>
{
    public Task<ITaskCommand<CraneTaskModel>> ExecuteAsync(CraneTaskModel model, CancellationToken token)
    {
        Console.WriteLine($"[{model.Code}] 🚶 设备走行 - 第{model.StepCounter++}步");
        return Task.FromResult<ITaskCommand<CraneTaskModel>>(new CraneGet());
    }
}

public class CraneGet : ITaskCommand<CraneTaskModel>
{
    public Task<ITaskCommand<CraneTaskModel>> ExecuteAsync(CraneTaskModel model, CancellationToken token)
    {
        Console.WriteLine($"[{model.Code}] 🤏 抓料中 - 第{model.StepCounter++}步");
        return Task.FromResult<ITaskCommand<CraneTaskModel>>(new CranePut());
    }
}

public class CranePut : ITaskCommand<CraneTaskModel>
{
    public Task<ITaskCommand<CraneTaskModel>> ExecuteAsync(CraneTaskModel model, CancellationToken token)
    {
        Console.WriteLine($"[{model.Code}] 📤 放料中 - 第{model.StepCounter++}步");
        return Task.FromResult<ITaskCommand<CraneTaskModel>>(new CraneFinishOrder());
    }
}

public class CraneFinishOrder : ITaskCommand<CraneTaskModel>
{
    public Task<ITaskCommand<CraneTaskModel>> ExecuteAsync(CraneTaskModel model, CancellationToken token)
    {
        Console.WriteLine($"[{model.Code}] ✅ 任务完成，共执行 {model.StepCounter} 步");
        model.StepCounter = 0; // 重置
        return Task.FromResult<ITaskCommand<CraneTaskModel>>(new CraneVerifyOrder()); // 循环测试
    }
}

// === 调度管理器 ===

public class TaskManager
{
    private readonly Dictionary<string, CraneTaskModel> _taskMap = new();
    private readonly Dictionary<string, CancellationTokenSource> _cancellationMap = new();

    public void InitTasks(List<string> deviceCodes)
    {
        foreach (var code in deviceCodes)
        {
            var model = new CraneTaskModel
            {
                Code = code,
                TaskCommand = new CraneVerifyOrder()
            };
            _taskMap[code] = model;

            var cts = new CancellationTokenSource();
            _cancellationMap[code] = cts;

            StartTaskLoop(model, cts.Token);
        }
    }

    private void StartTaskLoop(CraneTaskModel model, CancellationToken token)
    {
        Console.WriteLine($"[{model.Code}] 🚀 启动任务线程");

        Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    model.TaskCommand = await model.TaskCommand.ExecuteAsync(model, token);
                    await Task.Delay(1000); // 每秒执行一次
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{model.Code}] ❌ 错误: {ex.Message}");
                    break;
                }
            }
        }, token);
    }

    public void StopAll()
    {
        foreach (var cts in _cancellationMap.Values)
        {
            cts.Cancel();
        }
    }
}

// === 程序入口点 ===

class Program
{
    static async Task Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("🛠 起重机任务系统启动中...");

        var taskManager = new TaskManager();
        taskManager.InitTasks(new List<string> { "Crane_01" });

        Console.WriteLine("✅ 正在运行，按任意键退出...");
        await Task.Run(() => Console.ReadKey());

        taskManager.StopAll();
        Console.WriteLine("🛑 所有任务已停止");
    }
}
