using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

namespace WebApplication1.AOP.Proxy
{
    public class ProxyFactory
    {
    }
}

//#region xx

//using System;
//using System.Collections.Concurrent;
//using System.Diagnostics;
//using System.Linq;
//using System.Reflection;
//using System.Threading.Tasks;

//// --------- 接口定义 ----------
//public interface IPerson
//{
//    Task AttackAsync(IPerson target, int damage);
//    Task UseSkillAsync(string skillName, IPerson target);
//    string Name { get; }
//}

//// --------- 业务实现 ----------
//public class Person : IPerson
//{
//    public string Name { get; }
//    private int _health = 100;
//    private static readonly Random Rand = new();

//    public Person(string name) => Name = name;

//    public async Task AttackAsync(IPerson target, int damage)
//    {
//        await Task.Delay(50); // 模拟耗时
//        Console.WriteLine($"{Name} 攻击了 {target.Name}，造成了 {damage} 点伤害。");
//        (target as Person)?.TakeDamage(damage);
//    }

//    public async Task UseSkillAsync(string skillName, IPerson target)
//    {
//        await Task.Delay(80);
//        int skillDamage = Rand.Next(10, 30);
//        Console.WriteLine($"{Name} 使用了技能 {skillName} 对 {target.Name} 造成了 {skillDamage} 点伤害。");
//        (target as Person)?.TakeDamage(skillDamage);
//    }

//    public void TakeDamage(int damage)
//    {
//        _health -= damage;
//        Console.WriteLine($"{Name} 剩余血量：{_health}");
//        if (_health <= 0)
//            Console.WriteLine($"{Name} 已被击败！");
//    }
//}

//// --------- 拦截器接口 ----------
//public interface IInterceptor
//{
//    Task<object> InterceptAsync(InvocationContext context, Func<Task<object>> next);
//}

//// --------- 调用上下文 ----------
//public class InvocationContext
//{
//    public object Target { get; }
//    public MethodInfo Method { get; }
//    public object[] Arguments { get; }

//    public InvocationContext(object target, MethodInfo method, object[] args)
//    {
//        Target = target;
//        Method = method;
//        Arguments = args;
//    }
//}

//// --------- 核心代理类 ----------
//public class UltimateProxy<T> : DispatchProxy where T : class
//{
//    public T Target { get; set; }
//    public IInterceptor[] Interceptors { get; set; }

//    protected override object Invoke(MethodInfo targetMethod, object[] args)
//    {
//        var context = new InvocationContext(Target, targetMethod, args);

//        // 构造拦截链
//        Func<Task<object>> invokeTarget = () => InvokeTargetAsync(context);

//        var pipeline = Interceptors.Reverse()
//            .Aggregate(invokeTarget, (next, interceptor) => () => interceptor.InterceptAsync(context, next));

//        var returnType = targetMethod.ReturnType;

//        if (typeof(Task).IsAssignableFrom(returnType))
//        {
//            var task = pipeline();

//            if (returnType == typeof(Task))
//            {
//                return task;
//            }
//            else
//            {
//                var resultType = returnType.GenericTypeArguments[0];
//                return ConvertTaskResult(task, resultType);
//            }
//        }
//        else
//        {
//            // 同步调用，直接等待执行结果
//            return pipeline().GetAwaiter().GetResult();
//        }
//    }

//    private async Task<object> InvokeTargetAsync(InvocationContext context)
//    {
//        try
//        {
//            var result = context.Method.Invoke(context.Target, context.Arguments);
//            if (result is Task task)
//            {
//                await task.ConfigureAwait(false);
//                if (context.Method.ReturnType.IsGenericType)
//                {
//                    var prop = task.GetType().GetProperty("Result");
//                    return prop.GetValue(task);
//                }
//                else
//                {
//                    return null;
//                }
//            }
//            else
//            {
//                return result;
//            }
//        }
//        catch (TargetInvocationException tie)
//        {
//            throw tie.InnerException;
//        }
//    }

//    private object ConvertTaskResult(Task<object> task, Type resultType)
//    {
//        var tcsType = typeof(TaskCompletionSource<>).MakeGenericType(resultType);
//        dynamic tcs = Activator.CreateInstance(tcsType);

//        task.ContinueWith(t =>
//        {
//            if (t.IsFaulted)
//                tcs.SetException(t.Exception.InnerExceptions);
//            else if (t.IsCanceled)
//                tcs.SetCanceled();
//            else
//                tcs.SetResult(Convert.ChangeType(t.Result, resultType));
//        });

//        return tcs.Task;
//    }
//}

//// --------- 缓存拦截器 ----------
//public class CacheInterceptor : IInterceptor
//{
//    private readonly ConcurrentDictionary<string, object> _cache = new();

//    public async Task<object> InterceptAsync(InvocationContext context, Func<Task<object>> next)
//    {
//        if (context.Method.Name.StartsWith("get_"))
//        {
//            string key = context.Method.Name;
//            if (_cache.TryGetValue(key, out var cached))
//            {
//                Console.WriteLine($"[缓存] 直接返回缓存的 {key}");
//                return cached;
//            }
//            var result = await next();
//            _cache[key] = result;
//            return result;
//        }
//        return await next();
//    }
//}

//// --------- 统计拦截器 ----------
//public class StatsInterceptor : IInterceptor
//{
//    private int _callCount = 0;
//    private long _totalElapsedMs = 0;
//    private int _exceptionCount = 0;
//    private Stopwatch _sw = new();

//    public async Task<object> InterceptAsync(InvocationContext context, Func<Task<object>> next)
//    {
//        _callCount++;
//        _sw.Restart();

//        try
//        {
//            var result = await next();
//            _sw.Stop();
//            _totalElapsedMs += _sw.ElapsedMilliseconds;

//            Console.WriteLine($"[统计] 方法 {context.Method.Name} 调用次数: {_callCount}, 平均耗时: {_totalElapsedMs / (double)_callCount:F2} ms, 异常次数: {_exceptionCount}");
//            return result;
//        }
//        catch (Exception)
//        {
//            _exceptionCount++;
//            _sw.Stop();
//            Console.WriteLine($"[统计] 方法 {context.Method.Name} 抛出异常! 累计异常次数: {_exceptionCount}");
//            throw;
//        }
//    }
//}

//// --------- 日志拦截器 ----------
//public class LoggingInterceptor : IInterceptor
//{
//    public async Task<object> InterceptAsync(InvocationContext context, Func<Task<object>> next)
//    {
//        string FormatArg(object arg)
//        {
//            if (arg is IPerson p) return p.Name;
//            if (arg == null) return "空";
//            return arg.ToString();
//        }

//        Console.WriteLine($"[日志] 调用方法: {context.Method.Name} 参数: {string.Join(", ", context.Arguments.Select(FormatArg))}");

//        try
//        {
//            var result = await next();
//            Console.WriteLine($"[日志] 方法 {context.Method.Name} 执行完成");
//            return result;
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"[日志] 方法 {context.Method.Name} 抛出异常: {ex.Message}");
//            throw;
//        }
//    }
//}

//// --------- 重试拦截器 ----------
//public class RetryInterceptor : IInterceptor
//{
//    private readonly int _maxRetries;
//    private readonly int _delayMilliseconds;

//    public RetryInterceptor(int maxRetries = 3, int delayMilliseconds = 100)
//    {
//        _maxRetries = maxRetries;
//        _delayMilliseconds = delayMilliseconds;
//    }

//    public async Task<object> InterceptAsync(InvocationContext context, Func<Task<object>> next)
//    {
//        int attempts = 0;
//        while (true)
//        {
//            try
//            {
//                attempts++;
//                return await next();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"[重试] 方法 {context.Method.Name} 第 {attempts} 次失败: {ex.Message}");
//                if (attempts >= _maxRetries) throw;
//                await Task.Delay(_delayMilliseconds);
//            }
//        }
//    }
//}

//// --------- 代理工厂 ----------
//public static class UltimateProxyFactory
//{
//    public static TInterface Create<TInterface>(TInterface target, params IInterceptor[] interceptors) where TInterface : class
//    {
//        if (!typeof(TInterface).IsInterface)
//            throw new ArgumentException("TInterface 必须是接口类型");

//        var proxy = DispatchProxy.Create<TInterface, UltimateProxy<TInterface>>();
//        var p = (UltimateProxy<TInterface>)(object)proxy;
//        p.Target = target;
//        p.Interceptors = interceptors;
//        return proxy;
//    }
//}

//// --------- 测试主程序 ----------
//class Program
//{
//    static async Task Main()
//    {
//        var personA = UltimateProxyFactory.Create<IPerson>(
//            new Person("PersonA"),
//            new CacheInterceptor(),
//            new StatsInterceptor(),
//            new LoggingInterceptor(),
//            new RetryInterceptor(maxRetries: 2, delayMilliseconds: 200)
//        );

//        var personB = UltimateProxyFactory.Create<IPerson>(
//            new Person("PersonB"),
//            new CacheInterceptor(),
//            new StatsInterceptor(),
//            new LoggingInterceptor(),
//            new RetryInterceptor()
//        );

//        await personA.AttackAsync(personB, 15);
//        await personB.UseSkillAsync("火球术", personA);

//        await personA.AttackAsync(personB, 20);
//        await personB.UseSkillAsync("冰刺", personA);
//    }
//}
//#endregion