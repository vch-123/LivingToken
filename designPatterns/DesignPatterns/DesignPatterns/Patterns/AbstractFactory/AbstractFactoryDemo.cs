using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns.AbstractFactory;

// ===== 抽象产品族 =====
public interface IButton
{
    void Render();
}

public interface ITextbox
{
    void Render();
}

// ===== Windows 系列产品 =====
public class WindowsButton : IButton
{
    public void Render() => Console.WriteLine("Render Windows Button");
}

public class WindowsTextbox : ITextbox
{
    public void Render() => Console.WriteLine("Render Windows Textbox");
}

// ===== Mac 系列产品 =====
public class MacButton : IButton
{
    public void Render() => Console.WriteLine("Render Mac Button");
}

public class MacTextbox : ITextbox
{
    public void Render() => Console.WriteLine("Render Mac Textbox");
}

// ===== 抽象工厂接口（产品族工厂）=====
public interface IGuiFactory
{
    IButton CreateButton();
    ITextbox CreateTextbox();
}

// ===== 具体工厂：Windows =====
public class WindowsFactory : IGuiFactory
{
    public IButton CreateButton() => new WindowsButton();
    public ITextbox CreateTextbox() => new WindowsTextbox();
}

// ===== 具体工厂：Mac =====
public class MacFactory : IGuiFactory
{
    public IButton CreateButton() => new MacButton();
    public ITextbox CreateTextbox() => new MacTextbox();
}

//using Patterns.AbstractFactory;

//Console.WriteLine("== 抽象工厂模式 Demo ==");

//// 使用 Windows 工厂
//IGuiFactory factory = new WindowsFactory();

//IButton button = factory.CreateButton();
//ITextbox textbox = factory.CreateTextbox();

//button.Render();
//textbox.Render();

//// 切换到 Mac 工厂
//factory = new MacFactory();
//button = factory.CreateButton();
//textbox = factory.CreateTextbox();

//button.Render();
//textbox.Render();
