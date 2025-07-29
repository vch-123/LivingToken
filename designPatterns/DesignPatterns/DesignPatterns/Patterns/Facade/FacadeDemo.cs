using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//定义：为复杂子系统提供一个统一的接口，让子系统更易用，降低客户端与子系统的耦合。
// 子系统A
class CPU
{
    public void Start() => Console.WriteLine("CPU 启动");
    public void Shutdown() => Console.WriteLine("CPU 关闭");
}

// 子系统B
class Memory
{
    public void Load() => Console.WriteLine("内存加载");
    public void Clear() => Console.WriteLine("内存清理");
}

// 子系统C
class Disk
{
    public void Read() => Console.WriteLine("硬盘读取数据");
    public void Write() => Console.WriteLine("硬盘写入数据");
}

// 外观类
class ComputerFacade
{
    private CPU _cpu = new();
    private Memory _memory = new();
    private Disk _disk = new();

    public void Start()
    {
        Console.WriteLine("电脑开始启动流程");
        _cpu.Start();
        _memory.Load();
        _disk.Read();
        Console.WriteLine("电脑启动完成");
    }

    public void Shutdown()
    {
        Console.WriteLine("电脑开始关闭流程");
        _disk.Write();
        _memory.Clear();
        _cpu.Shutdown();
        Console.WriteLine("电脑关闭完成");
    }
}



public class InventoryService
{
    public bool ReserveStock(int productId, int quantity)
    {
        Console.WriteLine($"库存系统：保留商品{productId}数量{quantity}");
        return true; // 简化，假设库存足够
    }
}
public class PaymentService
{
    public bool ProcessPayment(string userId, decimal amount)
    {
        Console.WriteLine($"支付系统：为用户{userId}扣款{amount}元");
        return true; // 简化，支付成功
    }
}
public class ShippingService
{
    public void ShipOrder(int orderId)
    {
        Console.WriteLine($"发货系统：订单{orderId}已发货");
    }
}
public class NotificationService
{
    public void NotifyUser(string userId, string message)
    {
        Console.WriteLine($"通知系统：通知用户{userId}，内容：{message}");
    }
}
public class OrderFacade
{
    private readonly InventoryService _inventoryService = new();
    private readonly PaymentService _paymentService = new();
    private readonly ShippingService _shippingService = new();
    private readonly NotificationService _notificationService = new();

    public bool PlaceOrder(int orderId, int productId, int quantity, string userId, decimal amount)
    {
        Console.WriteLine("订单处理开始");

        if (!_inventoryService.ReserveStock(productId, quantity))
        {
            Console.WriteLine("库存不足，订单失败");
            return false;
        }

        if (!_paymentService.ProcessPayment(userId, amount))
        {
            Console.WriteLine("支付失败，订单失败");
            return false;
        }

        _shippingService.ShipOrder(orderId);

        _notificationService.NotifyUser(userId, $"订单{orderId}已成功下单，商品{productId}已发货");

        Console.WriteLine("订单处理完成");
        return true;
    }
}
//class Program
//{
//    static void Main()
//    {
//        var orderFacade = new OrderFacade();

//        orderFacade.PlaceOrder(orderId: 1001, productId: 2002, quantity: 3, userId: "user123", amount: 299.99m);
//    }
//}
