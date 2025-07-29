using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//好的！我们现在来学习结构型设计模式中的经典代表——装饰器模式（Decorator Pattern）。

//🎯 装饰器模式简介
//目的：在不修改原类代码的情况下，动态地增强（扩展）对象的功能。

//换句话说，它是一种“包装”技术 —— 把对象包一层又一层，每一层都添加一点新功能。

//🧠 使用场景
//给对象动态添加功能，而不是通过继承。

//想要组合出灵活的功能，而不是固定死的子类。

//比如日志增强、权限控制、加密、缓冲、IO流的包裹等。


// 组件接口
public interface INotifier
{
    void Send(string message);
}

// 具体组件
public class EmailNotifier : INotifier
{
    public void Send(string message)
    {
        Console.WriteLine($"📧 Email sent: {message}");
    }
}

// 装饰器基类
public abstract class NotifierDecorator : INotifier
{
    protected INotifier _wrapped;
    public NotifierDecorator(INotifier notifier) => _wrapped = notifier;

    public virtual void Send(string message) => _wrapped.Send(message);
}

// 具体装饰器A：短信功能
public class SmsNotifier : NotifierDecorator
{
    public SmsNotifier(INotifier notifier) : base(notifier) { }

    public override void Send(string message)
    {
        base.Send(message);
        Console.WriteLine($"📱 SMS sent: {message}");
    }
}

// 具体装饰器B：微信功能
public class WeChatNotifier : NotifierDecorator
{
    public WeChatNotifier(INotifier notifier) : base(notifier) { }

    public override void Send(string message)
    {
        base.Send(message);
        Console.WriteLine($"💬 WeChat message sent: {message}");
    }
}


//class Program
//{
//    static void Main()
//    {
//        // 最基础：只发邮件
//        INotifier email = new EmailNotifier();

//        // 添加短信通知
//        INotifier emailWithSms = new SmsNotifier(email);

//        // 添加微信通知（= 邮件 + 短信 + 微信）
//        INotifier allNotify = new WeChatNotifier(emailWithSms);

//        allNotify.Send("系统报警：服务器宕机！");
//    }
//}


//INotifier notifier = new WechatNotifier(
//                         new SmsNotifier(
//                             new EmailNotifier()));

//notifier.Send("你好，这是多层通知");
