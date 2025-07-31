//命令模式（Command Pattern）
//定义
//命令模式是一种行为型设计模式，它将一个请求封装为一个对象，从而使用户可用不同的请求对客户进行参数化；对请求排队或记录请求日志，以及支持可撤销的操作。
//主要优点
//解耦
//命令模式将请求的发起者和执行者解耦，使得两者之间不需要直接调用。
//扩展性
//新的命令可以很容易地添加到系统中，而不需要修改现有的代码。
//支持撤销操作
//命令模式可以很容易地实现命令的撤销和重做功能。
//主要角色
//Command（命令接口）
//定义了执行操作的接口，具体命令类需要实现这个接口。
//ConcreteCommand（具体命令类）
//实现了命令接口，绑定了一个接收者，并调用接收者的业务逻辑。
//Client（客户端）
//创建具体命令对象，并设置其接收者。
//Invoker（调用者）
//要求命令对象执行请求。
//Receiver（接收者）
//知道如何实施与执行一个请求相关的操作。



public interface ICommand
{
    void Execute();
}

public class LightOnCommand : ICommand
{
    private Light _light;

    public LightOnCommand(Light light)
    {
        _light = light;
    }

    public void Execute()
    {
        _light.TurnOn();
    }
}

public class LightOffCommand : ICommand
{
    private Light _light;

    public LightOffCommand(Light light)
    {
        _light = light;
    }

    public void Execute()
    {
        _light.TurnOff();
    }
}

public class Light
{
    public void TurnOn()
    {
        Console.WriteLine("Light is ON");
    }

    public void TurnOff()
    {
        Console.WriteLine("Light is OFF");
    }
}

public class RemoteControll
{
    private ICommand _command;

    public void SetCommand(ICommand command)
    {
        _command = command;
    }

    public void PressButton()
    {
        _command.Execute();
    }
}

//public class Program
//{
//    public static void Main(string[] args)
//    {
//        // 创建接收者
//        Light light = new Light();

//        // 创建具体命令对象，并设置接收者
//        ICommand lightOnCommand = new LightOnCommand(light);
//        ICommand lightOffCommand = new LightOffCommand(light);

//        // 创建调用者，并设置命令
//        RemoteControll remoteControl = new RemoteControll();
//        remoteControl.SetCommand(lightOnCommand);
//        remoteControl.PressButton(); // 输出: Light is ON

//        remoteControl.SetCommand(lightOffCommand);
//        remoteControl.PressButton(); // 输出: Light is OFF
//    }
//}