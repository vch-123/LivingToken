using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;

//比如我们有很多不同的角色：战士、法师、弓箭手，现在我们想为他们做“导出为 JSON”和“导出为 XML”这两个功能。我们用访问者模式来实现，而不改动角色类本身。

// 元素接口
interface IGameCharacter
{
    void Accept(ICharacterVisitor visitor);
    string Name { get; }
}

// 访问者接口
interface ICharacterVisitor
{
    void Visit(Warrior warrior);
    void Visit(Mage mage);
    void Visit(Archer archer);
}

// 具体元素类：战士
class Warrior : IGameCharacter
{
    public string Name => "战士";
    public int Strength => 100;

    public void Accept(ICharacterVisitor visitor)
    {
        visitor.Visit(this);
    }
}

// 具体元素类：法师
class Mage : IGameCharacter
{
    public string Name => "法师";
    public int Mana => 200;

    public void Accept(ICharacterVisitor visitor)
    {
        visitor.Visit(this);
    }
}

// 具体元素类：弓箭手
class Archer : IGameCharacter
{
    public string Name => "弓箭手";
    public int Agility => 150;

    public void Accept(ICharacterVisitor visitor)
    {
        visitor.Visit(this);
    }
}

// 访问者实现：导出为 JSON
class JsonExportVisitor : ICharacterVisitor
{
    public void Visit(Warrior warrior)
    {
        Console.WriteLine($"{{\"type\":\"{warrior.Name}\",\"strength\":{warrior.Strength}}}");
    }

    public void Visit(Mage mage)
    {
        Console.WriteLine($"{{\"type\":\"{mage.Name}\",\"mana\":{mage.Mana}}}");
    }

    public void Visit(Archer archer)
    {
        Console.WriteLine($"{{\"type\":\"{archer.Name}\",\"agility\":{archer.Agility}}}");
    }
}

// 访问者实现：导出为 XML
class XmlExportVisitor : ICharacterVisitor
{
    public void Visit(Warrior warrior)
    {
        Console.WriteLine($"<character type=\"{warrior.Name}\"><strength>{warrior.Strength}</strength></character>");
    }

    public void Visit(Mage mage)
    {
        Console.WriteLine($"<character type=\"{mage.Name}\"><mana>{mage.Mana}</mana></character>");
    }

    public void Visit(Archer archer)
    {
        Console.WriteLine($"<character type=\"{archer.Name}\"><agility>{archer.Agility}</agility></character>");
    }
}

//// 主程序入口
//class Program
//{
//    static void Main()
//    {
//        var characters = new List<IGameCharacter>
//        {
//            new Warrior(),
//            new Mage(),
//            new Archer()
//        };

//        Console.WriteLine("=== JSON Export ===");
//        var jsonVisitor = new JsonExportVisitor();
//        characters.ForEach(c => c.Accept(jsonVisitor));

//        Console.WriteLine("\n=== XML Export ===");
//        var xmlVisitor = new XmlExportVisitor();
//        characters.ForEach(c => c.Accept(xmlVisitor));
//    }
//}

