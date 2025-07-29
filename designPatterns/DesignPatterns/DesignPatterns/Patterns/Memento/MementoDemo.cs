using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;

// 备忘录类，保存状态快照及元信息
public class Memento
{
    public string State { get; }
    public int Version { get; }
    public DateTime Timestamp { get; }

    public Memento(string state, int version)
    {
        State = state;
        Version = version;
        Timestamp = DateTime.Now;
    }

    public override string ToString() =>
        $"版本: {Version}, 时间: {Timestamp}, 状态: {State}";
}

// 发起人，拥有需要保存的状态
public class Originator
{
    public string State { get; set; }

    public Memento SaveState(int version) => new Memento(State, version);

    public void RestoreState(Memento memento)
    {
        State = memento.State;
    }
}

// 负责人，管理备忘录
public class Caretaker
{
    private readonly List<Memento> _history = new();
    private int _versionCounter = 0;

    // 保存当前状态，自动分配版本号
    public void Backup(Originator originator)
    {
        _versionCounter++;
        var memento = originator.SaveState(_versionCounter);
        _history.Add(memento);
        Console.WriteLine($"保存备忘录: {memento}");
    }

    // 根据版本号恢复状态
    public void Undo(Originator originator, int version)
    {
        var memento = _history.FindLast(m => m.Version == version);
        if (memento != null)
        {
            originator.RestoreState(memento);
            Console.WriteLine($"恢复到版本 {version}: {memento.State}");
        }
        else
        {
            Console.WriteLine($"未找到版本 {version} 的备忘录");
        }
    }

    // 恢复到上一个版本
    public void UndoLast(Originator originator)
    {
        if (_history.Count > 0)
        {
            var last = _history[_history.Count - 1];
            originator.RestoreState(last);
            _history.RemoveAt(_history.Count - 1);
            Console.WriteLine($"恢复到版本 {last.Version}: {last.State}");
        }
        else
        {
            Console.WriteLine("没有备忘录可以恢复");
        }
    }

    // 打印所有备忘录历史
    public void ShowHistory()
    {
        Console.WriteLine("备忘录历史:");
        foreach (var m in _history)
        {
            Console.WriteLine(m);
        }
    }
}


//class Program
//{
//    static void Main()
//    {
//        var originator = new Originator();
//        var caretaker = new Caretaker();

//        originator.State = "状态A";
//        caretaker.Backup(originator);

//        originator.State = "状态B";
//        caretaker.Backup(originator);

//        originator.State = "状态C";
//        caretaker.Backup(originator);

//        caretaker.ShowHistory();

//        caretaker.Undo(originator, 2);
//        Console.WriteLine($"当前状态: {originator.State}");

//        caretaker.UndoLast(originator);
//        Console.WriteLine($"当前状态: {originator.State}");
//    }
//}



