using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//🎯 享元模式简介
//目的：通过共享尽可能多的对象来减少内存占用，适用于大量细粒度对象的场景。

//🧠 何时用？
//系统中有成千上万个对象，造成内存压力。

//对象中有大量重复数据，可以共享。

//需要大量相似对象，且它们的大部分状态是可以共享的。

//💡 核心思想
//把对象状态分为两类：

//内部状态（Intrinsic State）：可以共享，不会随环境改变。

//外部状态（Extrinsic State）：不能共享，随环境变化，由外部传入。

//享元模式就是把内部状态共享，避免重复创建。




public interface ITreeType
{
    void Display(int x, int y);
}

public class TreeType : ITreeType
{
    private string _name;
    private string _color;
    private string _texture;

    public TreeType(string name, string color, string texture)
    {
        _name = name;
        _color = color;
        _texture = texture;
    }

    public void Display(int x, int y)
    {
        Console.WriteLine($"绘制树种 {_name}，颜色 {_color}，纹理 {_texture}，位置 ({x},{y})");
    }
}

public class TreeFactory
{
    private static Dictionary<string, TreeType> _treeTypes = new();

    public static TreeType GetTreeType(string name, string color, string texture)
    {
        string key = $"{name}_{color}_{texture}";
        if (!_treeTypes.ContainsKey(key))
        {
            _treeTypes[key] = new TreeType(name, color, texture);
        }
        return _treeTypes[key];
    }
}
public class Tree
{
    private int _x;
    private int _y;
    private TreeType _type;

    public Tree(int x, int y, TreeType type)
    {
        _x = x;
        _y = y;
        _type = type;
    }

    public void Draw()
    {
        _type.Display(_x, _y);
    }
}


//class Program
//{
//    static void Main()
//    {
//        var forest = new List<Tree>();

//        forest.Add(new Tree(10, 20, TreeFactory.GetTreeType("松树", "绿色", "粗糙")));
//        forest.Add(new Tree(15, 25, TreeFactory.GetTreeType("松树", "绿色", "粗糙")));
//        forest.Add(new Tree(30, 40, TreeFactory.GetTreeType("橡树", "深绿色", "光滑")));

//        foreach (var tree in forest)
//        {
//            tree.Draw();
//        }
//    }
//}
