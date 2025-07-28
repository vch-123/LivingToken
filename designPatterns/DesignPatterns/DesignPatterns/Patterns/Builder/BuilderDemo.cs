using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns.Builder;

// 产品类：Computer
public class Computer
{
    public string CPU { get; set; } = "";
    public string GPU { get; set; } = "";
    public string RAM { get; set; } = "";
    public string SSD { get; set; } = "";

    public void Show()
    {
        Console.WriteLine("Computer configuration:");
        Console.WriteLine($"CPU: {CPU}");
        Console.WriteLine($"GPU: {GPU}");
        Console.WriteLine($"RAM: {RAM}");
        Console.WriteLine($"SSD: {SSD}");
    }
}

// 抽象建造者接口
public interface IComputerBuilder
{
    void BuildCPU();
    void BuildGPU();
    void BuildRAM();
    void BuildSSD();
    Computer GetResult();
}

// 具体建造者：游戏电脑
public class GamingComputerBuilder : IComputerBuilder
{
    private readonly Computer _computer = new();

    public void BuildCPU() => _computer.CPU = "Intel i9";
    public void BuildGPU() => _computer.GPU = "NVIDIA RTX 4090";
    public void BuildRAM() => _computer.RAM = "32GB DDR5";
    public void BuildSSD() => _computer.SSD = "2TB NVMe";
    public Computer GetResult() => _computer;
}

// 具体建造者：办公电脑
public class OfficeComputerBuilder : IComputerBuilder
{
    private readonly Computer _computer = new();

    public void BuildCPU() => _computer.CPU = "Intel i5";
    public void BuildGPU() => _computer.GPU = "Integrated GPU";
    public void BuildRAM() => _computer.RAM = "16GB DDR4";
    public void BuildSSD() => _computer.SSD = "512GB SSD";
    public Computer GetResult() => _computer;
}

// 指挥者（可选）
public class Director
{
    public void Construct(IComputerBuilder builder)
    {
        builder.BuildCPU();
        builder.BuildGPU();
        builder.BuildRAM();
        builder.BuildSSD();
    }
}


//using Patterns.Builder;

//Console.WriteLine("== 建造者模式 Demo ==");

//var director = new Director();

//// 创建游戏电脑
//IComputerBuilder gamingBuilder = new GamingComputerBuilder();
//director.Construct(gamingBuilder);
//Computer gamingPC = gamingBuilder.GetResult();
//gamingPC.Show();

//Console.WriteLine();

//// 创建办公电脑
//IComputerBuilder officeBuilder = new OfficeComputerBuilder();
//director.Construct(officeBuilder);
//Computer officePC = officeBuilder.GetResult();
//officePC.Show();


//# 建造者模式（Builder Pattern）项目实战应用

//建造者模式适用于：** 构建过程复杂、参数众多、组合变化多的对象构建**。

//---

//## ✅ 常见项目场景应用

//### 💡 1. ASP.NET Core 中的 `HttpClient` 注册（Fluent Builder 风格）

//```csharp
//builder.Services.AddHttpClient("github", client =>
//{
//    client.BaseAddress = new Uri("https://api.github.com/");
//    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
//});
//var query = new SqlBuilder()
//    .Select("Name, Age")
//    .From("Users")
//    .Where("Age > 18")
//    .OrderBy("Age DESC")
//    .Build();

//Console.WriteLine(query);
//// 输出：SELECT Name, Age FROM Users WHERE Age > 18 ORDER BY Age DESC
//export default defineConfig({
//plugins: [vue()],
//  server:
//    {
//    port: 3000,
//    proxy: { "/api": "http://localhost:5000" }
//    }
//});
//var builder = new WarriorBuilder();
//builder.WithHelmet("Iron Helmet")
//       .WithWeapon("Great Sword")
//       .WithArmor("Steel Armor")
//       .WithLevel(30);

//var warrior = builder.Build();
//var email = new EmailBuilder()
//    .To("user@example.com")
//    .Subject("欢迎加入！")
//    .Body("你好，这是欢迎邮件。")
//    .AddAttachment("说明书.pdf")
//    .Build();
