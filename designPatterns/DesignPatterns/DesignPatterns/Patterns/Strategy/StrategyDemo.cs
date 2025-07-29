//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.Extensions.DependencyInjection;

//// ==========================
//// 1️⃣ 支付类型枚举
//// ==========================
//public enum PaymentType
//{
//    WeChat,
//    Alipay,
//    UnionPay
//}

//// ==========================
//// 2️⃣ 支付策略接口
//// ==========================
//public interface IPaymentStrategy
//{
//    void Pay(decimal amount);           // 支付行为
//    PaymentType Type { get; }           // 用于识别策略类型
//}

//// ==========================
//// 3️⃣ 具体策略实现：微信支付
//// ==========================
//public class WeChatPay : IPaymentStrategy
//{
//    public PaymentType Type => PaymentType.WeChat;

//    public void Pay(decimal amount)
//    {
//        Console.WriteLine($"[微信支付] 成功支付 ￥{amount}");
//    }
//}

//// ==========================
//// 4️⃣ 具体策略实现：支付宝
//// ==========================
//public class Alipay : IPaymentStrategy
//{
//    public PaymentType Type => PaymentType.Alipay;

//    public void Pay(decimal amount)
//    {
//        Console.WriteLine($"[支付宝支付] 成功支付 ￥{amount}");
//    }
//}

//// ==========================
//// 5️⃣ 具体策略实现：银联
//// ==========================
//public class UnionPay : IPaymentStrategy
//{
//    public PaymentType Type => PaymentType.UnionPay;

//    public void Pay(decimal amount)
//    {
//        Console.WriteLine($"[银联支付] 成功支付 ￥{amount}");
//    }
//}

//// ==========================
//// 6️⃣ 支付策略工厂：根据类型获取策略
//// ==========================
//public class PaymentStrategyFactory
//{
//    private readonly Dictionary<PaymentType, IPaymentStrategy> _strategies;

//    // 所有策略通过构造函数注入进来，形成字典映射
//    public PaymentStrategyFactory(IEnumerable<IPaymentStrategy> strategies)
//    {
//        _strategies = strategies.ToDictionary(s => s.Type, s => s);
//    }

//    // 根据类型获取对应的策略对象
//    public IPaymentStrategy GetStrategy(PaymentType type)
//    {
//        if (_strategies.TryGetValue(type, out var strategy))
//            return strategy;

//        throw new NotSupportedException($"不支持的支付类型：{type}");
//    }
//}

//// ==========================
//// 7️⃣ Order 类：上下文对象，处理支付流程
//// ==========================
//public class Order
//{
//    public decimal TotalAmount { get; }
//    private readonly PaymentStrategyFactory _strategyFactory;

//    public Order(decimal totalAmount, PaymentStrategyFactory factory)
//    {
//        TotalAmount = totalAmount;
//        _strategyFactory = factory;
//    }

//    // 调用策略完成支付
//    public void Checkout(PaymentType type)
//    {
//        Console.WriteLine($"\n订单金额：￥{TotalAmount}，使用方式：{type}");
//        var strategy = _strategyFactory.GetStrategy(type);
//        strategy.Pay(TotalAmount);
//    }
//}

//// ==========================
//// 8️⃣ 程序入口：配置 DI、测试调用
//// ==========================
//class Program
//{
//    static void Main()
//    {
//        // 配置依赖注入容器
//        var services = new ServiceCollection();

//        // 注册所有支付策略
//        services.AddSingleton<IPaymentStrategy, WeChatPay>();
//        services.AddSingleton<IPaymentStrategy, Alipay>();
//        services.AddSingleton<IPaymentStrategy, UnionPay>();

//        // 注册策略工厂
//        services.AddSingleton<PaymentStrategyFactory>();

//        // 构建服务提供器
//        var provider = services.BuildServiceProvider();

//        // 获取策略工厂
//        var factory = provider.GetRequiredService<PaymentStrategyFactory>();

//        // 创建订单，模拟三种支付方式
//        var order = new Order(259.90m, factory);

//        order.Checkout(PaymentType.WeChat);    // 使用微信支付
//        order.Checkout(PaymentType.Alipay);    // 使用支付宝支付
//        order.Checkout(PaymentType.UnionPay);  // 使用银联支付
//    }
//}
