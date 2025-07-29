using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IDevice
{
    void TurnOn();
    void TurnOff();
    void SetChannel(int channel);
}

public class SonyTV : IDevice
{
    public void TurnOn() => Console.WriteLine("Sony TV is ON");
    public void TurnOff() => Console.WriteLine("Sony TV is OFF");
    public void SetChannel(int channel) => Console.WriteLine($"Sony TV: Channel set to {channel}");
}

public class SamsungTV : IDevice
{
    public void TurnOn() => Console.WriteLine("Samsung TV is ON");
    public void TurnOff() => Console.WriteLine("Samsung TV is OFF");
    public void SetChannel(int channel) => Console.WriteLine($"Samsung TV: Channel set to {channel}");
}

public abstract class RemoteControl
{
    protected IDevice _device;

    public RemoteControl(IDevice device)
    {
        _device = device;
    }

    public abstract void TurnOn();
    public abstract void TurnOff();
    public abstract void SetChannel(int channel);
}

public class AdvancedRemoteControl : RemoteControl
{
    public AdvancedRemoteControl(IDevice device) : base(device) { }

    public override void TurnOn() => _device.TurnOn();
    public override void TurnOff() => _device.TurnOff();
    public override void SetChannel(int channel) => _device.SetChannel(channel);
}



//class Program
//{
//    static void Main()
//    {
//        IDevice sony = new SonyTV();
//        RemoteControl remote = new AdvancedRemoteControl(sony);
//        remote.TurnOn();
//        remote.SetChannel(5);
//        remote.TurnOff();

//        Console.WriteLine();

//        IDevice samsung = new SamsungTV();
//        remote = new AdvancedRemoteControl(samsung);
//        remote.TurnOn();
//        remote.SetChannel(99);
//        remote.TurnOff();
//    }
//}
