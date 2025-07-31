//namespace WebApplication1.AOP
//{
//    public class Something
//    {
//    }
//}

#region 委托和事件

/*//委托的常见用法
//函数回调

Student student = new Student(){Name = "LiMing"};
Action action = null;
action += student.SayMyName;
action += student.SayTime;
student.DoSomething(DoSomethingEnum.Walk, action);

public enum DoSomethingEnum
{
    Walk,
    Run,
    Smile
}

public class Student
{
    public string Name{get;set;}

    public void SayMyName()
    {
        Console.WriteLine($"my name is {Name}");
    }

    public void SayTime()
    {
        Console.WriteLine($"Time is {DateTime.Now}");
    }

    public void DoSomething(DoSomethingEnum doSomethingEnum, Action action)
    {
        Console.WriteLine($"do something is {doSomethingEnum.ToString()}");
        action();
    }
}*/





/*public class Publisher
{
    // 定义事件，使用 Func<string, string> 委托
    public event Func<string, string> OnMessage;

    // 触发事件的方法，接收返回值并打印
    public void SendMessage(string message)
    {
        Console.WriteLine("发布者 SendMessage");
        string result = OnMessage?.Invoke(message); // 调用事件并获取返回值
        if (result != null)
        {
            Console.WriteLine("发布者收到订阅者的返回值: " + result);
        }
    }
}
public class Subscriber
{
    public string HandleMessage(string message)
    {
        Console.WriteLine("Subscriber received: " + message);
        return "Hello from Subscriber"; // 返回一个字符串
    }
}
public class Program
{
    public static void Main(string[] args)
    {
        Publisher publisher = new Publisher();
        Subscriber subscriber = new Subscriber();

        // 订阅事件
        publisher.OnMessage += subscriber.HandleMessage;

        // 触发事件
        publisher.SendMessage("Hello, World!");
    }
}*/

#endregion

//#region 匿名方法
//public class Program
//{
//    public static void Main(string[] args)
//    {
//        // 使用 Lambda 表达式
//        Action<string> printAction = message =>
//        {
//            Console.WriteLine(message);
//            Console.WriteLine(message);
//            int a = 3;
//            int b = 4;
//            Console.WriteLine(a + b);
//        };
//        printAction("Hello, World!");

//        // 使用 Lambda 表达式定义 Func
//        Func<int, int> squareFunc = number => number * number;
//        Console.WriteLine(squareFunc(5)); // 输出 25
//    }
//}
//#endregion

/*#region 反射实现事件绑定
using System;
using System.Reflection;

public class Publisher
{
    public event Action<string> OnMessage;

    public void SendMessage(string message)
    {
        OnMessage?.Invoke(message);
    }
}

public class Subscriber
{
    public void HandleMessage(string message)
    {
        Console.WriteLine("Subscriber received: " + message);
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        Publisher publisher = new Publisher();
        Subscriber subscriber = new Subscriber();

        // 使用反射动态绑定事件
        EventInfo eventInfo = typeof(Publisher).GetEvent("OnMessage");
        MethodInfo methodInfo = typeof(Subscriber).GetMethod("HandleMessage");
        Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, subscriber, methodInfo);
        eventInfo.AddEventHandler(publisher, handler);

        publisher.SendMessage("Hello, World!");
    }
}
#endregion*/