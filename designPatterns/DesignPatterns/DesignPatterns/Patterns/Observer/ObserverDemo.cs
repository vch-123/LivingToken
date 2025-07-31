
//首先说一下 观察者模式可以用事件实现，下面是用接口实现的示例




public interface IObserver
{
    void Update(string message);
}
public class Subject
{
    // 观察者列表
    private List<IObserver> _observers = new List<IObserver>();

    // 添加观察者
    public void Attach(IObserver observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    // 移除观察者
    public void Detach(IObserver observer)
    {
        _observers.Remove(observer);
    }

    // 通知所有观察者
    public void Notify(string message)
    {
        foreach (var observer in _observers)
        {
            observer.Update(message);
        }
    }
}

public class ConcreteObserver : IObserver
{
    public void Update(string message)
    {
        Console.WriteLine($"ConcreteObserver received: {message}");
    }
}

//public class Program
//{
//    public static void Main(string[] args)
//    {
//        // 创建被观察者
//        Subject subject = new Subject();

//        // 创建观察者
//        ConcreteObserver observer1 = new ConcreteObserver();
//        ConcreteObserver observer2 = new ConcreteObserver();

//        // 将观察者绑定到被观察者
//        subject.Attach(observer1);
//        subject.Attach(observer2);

//        // 通知观察者
//        subject.Notify("Hello, World!");

//        // 移除一个观察者
//        subject.Detach(observer1);

//        // 再次通知观察者
//        subject.Notify("Hello again!");
//    }
//}