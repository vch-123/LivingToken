class Program
{
    static void Main()
    {
        IDevice sony = new SonyTV();
        RemoteControl remote = new AdvancedRemoteControl(sony);
        remote.TurnOn();
        remote.SetChannel(5);
        remote.TurnOff();

        Console.WriteLine();

        IDevice samsung = new SamsungTV();
        remote = new AdvancedRemoteControl(samsung);
        remote.TurnOn();
        remote.SetChannel(99);
        remote.TurnOff();
    }
}
