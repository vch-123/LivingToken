//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using System.Text;

//// ================= 接口定义 =================
//public interface ITaskModel
//{
//    string Code { get; set; }
//}

//public interface ITaskCommand<T> where T : ITaskModel
//{
//    Task<ITaskCommand<T>> ExecuteAsync(T model, CancellationToken token);
//}

//// ================= 模型定义 =================
//public class CraneTaskModel : ITaskModel
//{
//    public string Code { get; set; }
//    public ITaskCommand<CraneTaskModel> TaskCommand { get; set; }
//    public int StepCounter { get; set; } = 0;
//}

//// ================= 日志服务 =================
//public interface IDeviceLogger
//{
//    void Log(string deviceCode, string commandName);
//}

//public class DeviceLogger : IDeviceLogger
//{
//    private readonly object _lock = new();

//    public void Log(string deviceCode, string commandName)
//    {
//        var now = DateTime.Now;
//        var logDir = Path.Combine(AppContext.BaseDirectory, "logs");
//        Directory.CreateDirectory(logDir);

//        var fileName = $"{deviceCode}_{now:yyyyMMdd}.log";
//        var filePath = Path.Combine(logDir, fileName);
//        var logLine = $"{now:yyyy-MM-dd HH:mm:ss} | {commandName}";

//        lock (_lock)
//        {
//            File.AppendAllText(filePath, logLine + Environment.NewLine, Encoding.UTF8);
//        }
//    }
//}

//// ================= 辅助类 =================
//public static class SleepHelper
//{
//    public static Task DelayRandom(int min = 100, int max = 300)
//    {
//        return Task.Delay(Random.Shared.Next(min, max));
//    }
//}

//// ================= 动态选择器 =================
//public class CommandSelector
//{
//    private readonly IServiceProvider _provider;
//    private readonly List<string> commandNames = new() { "Command1", "Command2", "Command3", "Command4" };

//    public CommandSelector(IServiceProvider provider)
//    {
//        _provider = provider;
//    }

//    public ITaskCommand<CraneTaskModel> GetNext(string current)
//    {
//        return current switch
//        {
//            "CraneVerifyOrder" => GetRandomCommand(),
//            "Command1" or "Command2" or "Command3" or "Command4" =>
//                Random.Shared.Next(100) < 70 ? GetRandomCommand() : Get<CraneFinishOrder>(),
//            "CraneFinishOrder" => Get<CraneVerifyOrder>(),
//            _ => throw new InvalidOperationException("未知命令")
//        };
//    }

//    private ITaskCommand<CraneTaskModel> GetRandomCommand()
//    {
//        var next = commandNames[Random.Shared.Next(commandNames.Count)];
//        return next switch
//        {
//            "Command1" => Get<Command1>(),
//            "Command2" => Get<Command2>(),
//            "Command3" => Get<Command3>(),
//            "Command4" => Get<Command4>(),
//            _ => throw new InvalidOperationException("未知命令")
//        };
//    }

//    private T Get<T>() where T : ITaskCommand<CraneTaskModel>
//        => (T)_provider.GetRequiredService(typeof(T));
//}

//// ================= 命令实现 =================

//public class CraneVerifyOrder : ITaskCommand<CraneTaskModel>
//{
//    private readonly IDeviceLogger _logger;
//    private readonly CommandSelector _selector;

//    public CraneVerifyOrder(IDeviceLogger logger, CommandSelector selector)
//    {
//        _logger = logger;
//        _selector = selector;
//    }

//    public async Task<ITaskCommand<CraneTaskModel>> ExecuteAsync(CraneTaskModel model, CancellationToken token)
//    {
//        await SleepHelper.DelayRandom();
//        _logger.Log(model.Code, nameof(CraneVerifyOrder));
//        Console.WriteLine($"[{model.Code}] ✅ 验证任务 - 第{model.StepCounter++}步");
//        return _selector.GetNext(nameof(CraneVerifyOrder));
//    }
//}

//public class CraneFinishOrder : ITaskCommand<CraneTaskModel>
//{
//    private readonly IDeviceLogger _logger;
//    private readonly CommandSelector _selector;

//    public CraneFinishOrder(IDeviceLogger logger, CommandSelector selector)
//    {
//        _logger = logger;
//        _selector = selector;
//    }

//    public async Task<ITaskCommand<CraneTaskModel>> ExecuteAsync(CraneTaskModel model, CancellationToken token)
//    {
//        await SleepHelper.DelayRandom();
//        _logger.Log(model.Code, nameof(CraneFinishOrder));
//        Console.WriteLine($"[{model.Code}] 🏁 任务完成，计数归零");
//        Console.WriteLine();
//        model.StepCounter = 0;
//        return _selector.GetNext(nameof(CraneFinishOrder));
//    }
//}

//public class Command1 : ITaskCommand<CraneTaskModel>
//{
//    private readonly IDeviceLogger _logger;
//    private readonly CommandSelector _selector;

//    public Command1(IDeviceLogger logger, CommandSelector selector)
//    {
//        _logger = logger;
//        _selector = selector;
//    }

//    public async Task<ITaskCommand<CraneTaskModel>> ExecuteAsync(CraneTaskModel model, CancellationToken token)
//    {
//        await SleepHelper.DelayRandom();
//        _logger.Log(model.Code, nameof(Command1));
//        Console.WriteLine($"[{model.Code}] 🛠 执行 Command1 - 第{model.StepCounter++}步");
//        return _selector.GetNext(nameof(Command1));
//    }
//}

//public class Command2 : ITaskCommand<CraneTaskModel>
//{
//    private readonly IDeviceLogger _logger;
//    private readonly CommandSelector _selector;

//    public Command2(IDeviceLogger logger, CommandSelector selector)
//    {
//        _logger = logger;
//        _selector = selector;
//    }

//    public async Task<ITaskCommand<CraneTaskModel>> ExecuteAsync(CraneTaskModel model, CancellationToken token)
//    {
//        await SleepHelper.DelayRandom();
//        _logger.Log(model.Code, nameof(Command2));
//        Console.WriteLine($"[{model.Code}] 🛠 执行 Command2 - 第{model.StepCounter++}步");
//        return _selector.GetNext(nameof(Command2));
//    }
//}

//public class Command3 : ITaskCommand<CraneTaskModel>
//{
//    private readonly IDeviceLogger _logger;
//    private readonly CommandSelector _selector;

//    public Command3(IDeviceLogger logger, CommandSelector selector)
//    {
//        _logger = logger;
//        _selector = selector;
//    }

//    public async Task<ITaskCommand<CraneTaskModel>> ExecuteAsync(CraneTaskModel model, CancellationToken token)
//    {
//        await SleepHelper.DelayRandom();
//        _logger.Log(model.Code, nameof(Command3));
//        Console.WriteLine($"[{model.Code}] 🛠 执行 Command3 - 第{model.StepCounter++}步");
//        return _selector.GetNext(nameof(Command3));
//    }
//}

//public class Command4 : ITaskCommand<CraneTaskModel>
//{
//    private readonly IDeviceLogger _logger;
//    private readonly CommandSelector _selector;

//    public Command4(IDeviceLogger logger, CommandSelector selector)
//    {
//        _logger = logger;
//        _selector = selector;
//    }

//    public async Task<ITaskCommand<CraneTaskModel>> ExecuteAsync(CraneTaskModel model, CancellationToken token)
//    {
//        await SleepHelper.DelayRandom();
//        _logger.Log(model.Code, nameof(Command4));
//        Console.WriteLine($"[{model.Code}] 🛠 执行 Command4 - 第{model.StepCounter++}步");
//        return _selector.GetNext(nameof(Command4));
//    }
//}

//// ================= 任务管理器 =================
//public class TaskManager
//{
//    private readonly IServiceProvider _provider;
//    private readonly Dictionary<string, CancellationTokenSource> _cancellationMap = new();

//    public TaskManager(IServiceProvider provider)
//    {
//        _provider = provider;
//    }

//    public void InitTasks(List<string> codes)
//    {
//        foreach (var code in codes)
//        {
//            var model = new CraneTaskModel
//            {
//                Code = code,
//                TaskCommand = _provider.GetRequiredService<CraneVerifyOrder>()
//            };

//            var cts = new CancellationTokenSource();
//            _cancellationMap[code] = cts;

//            StartLoop(model, cts.Token);
//        }
//    }

//    private void StartLoop(CraneTaskModel model, CancellationToken token)
//    {
//        Task.Run(async () =>
//        {
//            Console.WriteLine($"[{model.Code}] 🎬 启动任务");

//            while (!token.IsCancellationRequested)
//            {
//                try
//                {
//                    model.TaskCommand = await model.TaskCommand.ExecuteAsync(model, token);
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"[{model.Code}] ❌ 错误: {ex.Message}");
//                    break;
//                }

//                await Task.Delay(100);
//            }

//        }, token);
//    }

//    public void StopAll()
//    {
//        foreach (var cts in _cancellationMap.Values)
//        {
//            cts.Cancel();
//        }
//    }
//}

//// ================= 启动入口 =================


//class Program
//{
//    public static async Task Main(string[] args)
//    {
//        Console.OutputEncoding = Encoding.UTF8;
//        Console.WriteLine("🚀 Crane Task System 启动中...");

//        var host = Host.CreateDefaultBuilder(args)
//            .ConfigureServices(services =>
//            {
//                services.AddSingleton<IDeviceLogger, DeviceLogger>();
//                services.AddSingleton<CommandSelector>();

//                services.AddTransient<CraneVerifyOrder>();
//                services.AddTransient<CraneFinishOrder>();
//                services.AddTransient<Command1>();
//                services.AddTransient<Command2>();
//                services.AddTransient<Command3>();
//                services.AddTransient<Command4>();

//                services.AddSingleton<TaskManager>();
//            })
//            .Build();

//        var manager = host.Services.GetRequiredService<TaskManager>();
//        manager.InitTasks(new() { "Crane_01","Crane_02","Crane_03","Crane04" });

//        Console.WriteLine("✅ 系统运行中，按任意键退出...");
//        Console.ReadKey();

//        manager.StopAll();
//        Console.WriteLine("🛑 所有任务已停止");

//        await host.StopAsync();
//    }
//}


using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text;

// ================= 接口定义 =================
public interface ITaskModel
{
    string Code { get; set; }
}

public interface ITaskCommand<T> where T : ITaskModel
{
    Task<ITaskCommand<T>> ExecuteAsync(T model, CancellationToken token);
}

// ================= 模型定义 =================
public class CraneTaskModel : ITaskModel
{
    public string Code { get; set; }
    public ITaskCommand<CraneTaskModel> TaskCommand { get; set; }
    public int StepCounter { get; set; } = 0;
}

// ================= 日志服务 =================
public interface IDeviceLogger
{
    void Log(string deviceCode, string commandName);
}

public class DeviceLogger : IDeviceLogger
{
    private readonly object _lock = new();

    public void Log(string deviceCode, string commandName)
    {
        var now = DateTime.Now;
        var logDir = Path.Combine(AppContext.BaseDirectory, "logs");
        Directory.CreateDirectory(logDir);

        var fileName = $"{deviceCode}_{now:yyyyMMdd}.log";
        var filePath = Path.Combine(logDir, fileName);
        var logLine = $"{now:yyyy-MM-dd HH:mm:ss} | {commandName}";

        lock (_lock)
        {
            File.AppendAllText(filePath, logLine + Environment.NewLine, Encoding.UTF8);
        }
    }
}

// ================= 辅助类 =================
public static class SleepHelper
{
    public static Task DelayRandom(CancellationToken token, int min = 100, int max = 300)
    {
        return Task.Delay(Random.Shared.Next(min, max), token);
    }
}

// ================= 动态选择器 =================
public class CommandSelector
{
    private readonly IServiceProvider _provider;
    private readonly List<string> commandNames = new() { "Command1", "Command2", "Command3", "Command4" };

    public CommandSelector(IServiceProvider provider)
    {
        _provider = provider;
    }

    public ITaskCommand<CraneTaskModel> GetNext(string current)
    {
        return current switch
        {
            "CraneVerifyOrder" => GetRandomCommand(),
            "Command1" or "Command2" or "Command3" or "Command4" =>
                Random.Shared.Next(100) < 70 ? GetRandomCommand() : Get<CraneFinishOrder>(),
            "CraneFinishOrder" => Get<CraneVerifyOrder>(),
            _ => throw new InvalidOperationException("未知命令")
        };
    }

    private ITaskCommand<CraneTaskModel> GetRandomCommand()
    {
        var next = commandNames[Random.Shared.Next(commandNames.Count)];
        return next switch
        {
            "Command1" => Get<Command1>(),
            "Command2" => Get<Command2>(),
            "Command3" => Get<Command3>(),
            "Command4" => Get<Command4>(),
            _ => throw new InvalidOperationException("未知命令")
        };
    }

    private T Get<T>() where T : ITaskCommand<CraneTaskModel>
        => (T)_provider.GetRequiredService(typeof(T));
}

// ================= 命令实现 =================

public class CraneVerifyOrder : ITaskCommand<CraneTaskModel>
{
    private readonly IDeviceLogger _logger;
    private readonly CommandSelector _selector;

    public CraneVerifyOrder(IDeviceLogger logger, CommandSelector selector)
    {
        _logger = logger;
        _selector = selector;
    }

    public async Task<ITaskCommand<CraneTaskModel>> ExecuteAsync(CraneTaskModel model, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        await SleepHelper.DelayRandom(token);
        _logger.Log(model.Code, nameof(CraneVerifyOrder));
        Console.WriteLine($"[{model.Code}] ✅ 验证任务 - 第{model.StepCounter++}步");
        return _selector.GetNext(nameof(CraneVerifyOrder));
    }
}

public class CraneFinishOrder : ITaskCommand<CraneTaskModel>
{
    private readonly IDeviceLogger _logger;
    private readonly CommandSelector _selector;

    public CraneFinishOrder(IDeviceLogger logger, CommandSelector selector)
    {
        _logger = logger;
        _selector = selector;
    }

    public async Task<ITaskCommand<CraneTaskModel>> ExecuteAsync(CraneTaskModel model, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        await SleepHelper.DelayRandom(token);
        _logger.Log(model.Code, nameof(CraneFinishOrder));
        Console.WriteLine($"[{model.Code}] 🏁 任务完成，计数归零");
        Console.WriteLine();
        model.StepCounter = 0;
        return _selector.GetNext(nameof(CraneFinishOrder));
    }
}

public class Command1 : ITaskCommand<CraneTaskModel>
{
    private readonly IDeviceLogger _logger;
    private readonly CommandSelector _selector;

    public Command1(IDeviceLogger logger, CommandSelector selector)
    {
        _logger = logger;
        _selector = selector;
    }

    public async Task<ITaskCommand<CraneTaskModel>> ExecuteAsync(CraneTaskModel model, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        await SleepHelper.DelayRandom(token);
        _logger.Log(model.Code, nameof(Command1));
        Console.WriteLine($"[{model.Code}] 🛠 执行 Command1 - 第{model.StepCounter++}步");
        return _selector.GetNext(nameof(Command1));
    }
}

public class Command2 : ITaskCommand<CraneTaskModel>
{
    private readonly IDeviceLogger _logger;
    private readonly CommandSelector _selector;

    public Command2(IDeviceLogger logger, CommandSelector selector)
    {
        _logger = logger;
        _selector = selector;
    }

    public async Task<ITaskCommand<CraneTaskModel>> ExecuteAsync(CraneTaskModel model, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        await SleepHelper.DelayRandom(token);
        _logger.Log(model.Code, nameof(Command2));
        Console.WriteLine($"[{model.Code}] 🛠 执行 Command2 - 第{model.StepCounter++}步");
        return _selector.GetNext(nameof(Command2));
    }
}

public class Command3 : ITaskCommand<CraneTaskModel>
{
    private readonly IDeviceLogger _logger;
    private readonly CommandSelector _selector;

    public Command3(IDeviceLogger logger, CommandSelector selector)
    {
        _logger = logger;
        _selector = selector;
    }

    public async Task<ITaskCommand<CraneTaskModel>> ExecuteAsync(CraneTaskModel model, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        await SleepHelper.DelayRandom(token);
        _logger.Log(model.Code, nameof(Command3));
        Console.WriteLine($"[{model.Code}] 🛠 执行 Command3 - 第{model.StepCounter++}步");
        return _selector.GetNext(nameof(Command3));
    }
}

public class Command4 : ITaskCommand<CraneTaskModel>
{
    private readonly IDeviceLogger _logger;
    private readonly CommandSelector _selector;

    public Command4(IDeviceLogger logger, CommandSelector selector)
    {
        _logger = logger;
        _selector = selector;
    }

    public async Task<ITaskCommand<CraneTaskModel>> ExecuteAsync(CraneTaskModel model, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        await SleepHelper.DelayRandom(token);
        _logger.Log(model.Code, nameof(Command4));
        Console.WriteLine($"[{model.Code}] 🛠 执行 Command4 - 第{model.StepCounter++}步");
        return _selector.GetNext(nameof(Command4));
    }
}

// ================= 任务管理器 =================
public class TaskManager
{
    private readonly IServiceProvider _provider;
    private readonly Dictionary<string, CancellationTokenSource> _cancellationMap = new();

    public TaskManager(IServiceProvider provider)
    {
        _provider = provider;
    }

    public void InitTasks(List<string> codes)
    {
        foreach (var code in codes)
        {
            var model = new CraneTaskModel
            {
                Code = code,
                TaskCommand = _provider.GetRequiredService<CraneVerifyOrder>()
            };

            var cts = new CancellationTokenSource();
            _cancellationMap[code] = cts;

            StartLoop(model, cts.Token);
        }
    }

    private void StartLoop(CraneTaskModel model, CancellationToken token)
    {
        Task.Run(async () =>
        {
            Console.WriteLine($"[{model.Code}] 🎬 启动任务");

            try
            {
                while (!token.IsCancellationRequested)
                {
                    model.TaskCommand = await model.TaskCommand.ExecuteAsync(model, token);
                    await Task.Delay(100, token);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"[{model.Code}] ⛔ 已取消任务");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{model.Code}] ❌ 异常: {ex.Message}");
            }

        }, token);
    }

    public void StopTask(string code)
    {
        if (_cancellationMap.TryGetValue(code, out var cts))
        {
            cts.Cancel();
        }
    }

    public void StopAll()
    {
        foreach (var cts in _cancellationMap.Values)
        {
            cts.Cancel();
        }
    }
}

// ================= 启动入口 =================
class Program
{
    public static async Task Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("🚀 Crane Task System 启动中...");

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddSingleton<IDeviceLogger, DeviceLogger>();
                services.AddSingleton<CommandSelector>();

                services.AddTransient<CraneVerifyOrder>();
                services.AddTransient<CraneFinishOrder>();
                services.AddTransient<Command1>();
                services.AddTransient<Command2>();
                services.AddTransient<Command3>();
                services.AddTransient<Command4>();

                services.AddSingleton<TaskManager>();
            })
            .Build();

        var manager = host.Services.GetRequiredService<TaskManager>();
        manager.InitTasks(new() { "Crane_01" });

        Console.WriteLine("✅ 系统运行中，输入 crane 编号停止任务，例如输入 'Crane_01'");

        while (true)
        {
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) break;

            manager.StopTask(input.Trim());
        }

        manager.StopAll();
        Console.WriteLine("🛑 所有任务已停止");

        await host.StopAsync();
    }
}
