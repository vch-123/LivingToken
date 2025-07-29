using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//好！组合模式（Composite Pattern）是结构型设计模式中非常经典且实用的一个，
//它常用于处理 树形结构 —— 比如 UI 界面、组织架构、文件系统等。

// 抽象组件
public abstract class FileComponent
{
    public string Name { get; set; }

    public FileComponent(string name)
    {
        Name = name;
    }

    public abstract void Display(string indent = "");
}

// 叶子节点：文件
public class FileLeaf : FileComponent
{
    public FileLeaf(string name) : base(name) { }

    public override void Display(string indent = "")
    {
        Console.WriteLine($"{indent}📄 File: {Name}");
    }
}

// 容器节点：文件夹
public class FolderComposite : FileComponent
{
    private List<FileComponent> _children = new List<FileComponent>();

    public FolderComposite(string name) : base(name) { }

    public void Add(FileComponent component)
    {
        _children.Add(component);
    }

    public override void Display(string indent = "")
    {
        Console.WriteLine($"{indent}📁 Folder: {Name}");
        foreach (var child in _children)
        {
            child.Display(indent + "  ");
        }
    }
}


//class Program
//{
//    static void Main()
//    {
//        var root = new FolderComposite("Root");
//        root.Add(new FileLeaf("file1.txt"));
//        root.Add(new FileLeaf("file2.txt"));

//        var subFolder = new FolderComposite("SubFolder");
//        subFolder.Add(new FileLeaf("file3.txt"));
//        subFolder.Add(new FileLeaf("file4.txt"));

//        root.Add(subFolder);

//        root.Display();
//    }
//}
