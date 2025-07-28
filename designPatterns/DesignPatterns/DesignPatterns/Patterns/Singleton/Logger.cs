using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Patterns/Singleton/Logger.cs
namespace Patterns.Singleton;

public sealed class Logger
{
    private static Logger? _instance;
    private static readonly object _lock = new();

    // 私有构造函数，防止外部 new
    private Logger()
    {
        Console.WriteLine("Logger instance created.");
    }

    // 公共静态属性获取实例（线程安全）
    public static Logger Instance
    {
        get
        {
            lock (_lock)
            {
                _instance ??= new Logger();
                return _instance;
            }
        }
    }

    public void Log(string message)
    {
        Console.WriteLine($"[Log] {message}");
    }
}

//// Program.cs
//using Patterns.Singleton;

//Console.WriteLine("== 单例模式 Demo ==");

//var logger1 = Logger.Instance;
//logger1.Log("第一次调用");

//var logger2 = Logger.Instance;
//logger2.Log("第二次调用");

//Console.WriteLine($"logger1 和 logger2 是同一个实例？ {ReferenceEquals(logger1, logger2)}");

