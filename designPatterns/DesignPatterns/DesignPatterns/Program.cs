using System;
using System.Collections.Generic;
using System.Numerics; // 用 Vector3

// 角色状态
public class PlayerState
{
    public int HP { get; set; }
    public int MP { get; set; }
    public Vector3 Position { get; set; }
    public List<string> Inventory { get; set; } = new();

    public PlayerMemento Save() =>
        new PlayerMemento(HP, MP, Position, new List<string>(Inventory));

    public void Restore(PlayerMemento m)
    {
        HP = m.HP;
        MP = m.MP;
        Position = m.Position;
        Inventory = new List<string>(m.Inventory);
    }
}

public class PlayerMemento
{
    public int HP { get; }
    public int MP { get; }
    public Vector3 Position { get; }
    public List<string> Inventory { get; }

    public PlayerMemento(int hp, int mp, Vector3 pos, List<string> inv)
    {
        HP = hp;
        MP = mp;
        Position = pos;
        Inventory = inv;
    }
}

// 任务状态
public class QuestState
{
    public Dictionary<string, bool> CompletedQuests { get; set; } = new();

    public QuestMemento Save() =>
        new QuestMemento(new Dictionary<string, bool>(CompletedQuests));

    public void Restore(QuestMemento m)
    {
        CompletedQuests = new Dictionary<string, bool>(m.CompletedQuests);
    }
}

public class QuestMemento
{
    public Dictionary<string, bool> CompletedQuests { get; }

    public QuestMemento(Dictionary<string, bool> quests)
    {
        CompletedQuests = quests;
    }
}

// 地图状态
public class WorldState
{
    public List<string> DiscoveredAreas { get; set; } = new();

    public WorldMemento Save() =>
        new WorldMemento(new List<string>(DiscoveredAreas));

    public void Restore(WorldMemento m)
    {
        DiscoveredAreas = new List<string>(m.DiscoveredAreas);
    }
}

public class WorldMemento
{
    public List<string> DiscoveredAreas { get; }

    public WorldMemento(List<string> areas)
    {
        DiscoveredAreas = areas;
    }
}

// 统一的游戏存档备忘录，组合了所有子模块备忘录
public class GameSaveMemento
{
    public PlayerMemento Player { get; }
    public QuestMemento Quest { get; }
    public WorldMemento World { get; }

    public GameSaveMemento(PlayerMemento player, QuestMemento quest, WorldMemento world)
    {
        Player = player;
        Quest = quest;
        World = world;
    }
}

// 存档管理器，负责保存和恢复整个游戏状态
public class SaveManager
{
    private GameSaveMemento _save;

    public void SaveGame(PlayerState player, QuestState quest, WorldState world)
    {
        _save = new GameSaveMemento(
            player.Save(),
            quest.Save(),
            world.Save()
        );
        Console.WriteLine("游戏已保存！");
    }

    public void LoadGame(PlayerState player, QuestState quest, WorldState world)
    {
        if (_save == null)
        {
            Console.WriteLine("没有存档可加载！");
            return;
        }

        player.Restore(_save.Player);
        quest.Restore(_save.Quest);
        world.Restore(_save.World);
        Console.WriteLine("游戏已加载！");
    }
}

// 测试示例
class Program
{
    static void Main()
    {
        var player = new PlayerState
        {
            HP = 100,
            MP = 50,
            Position = new Vector3(10, 0, 20),
            Inventory = new List<string> { "Sword", "Shield" }
        };

        var quest = new QuestState();
        quest.CompletedQuests["Find the Ring"] = true;

        var world = new WorldState();
        world.DiscoveredAreas.Add("Limgrave");

        var saveManager = new SaveManager();

        // 保存游戏状态
        saveManager.SaveGame(player, quest, world);

        // 修改状态，模拟游戏继续
        player.HP = 40;
        player.Inventory.Remove("Shield");
        quest.CompletedQuests["Find the Ring"] = false;
        world.DiscoveredAreas.Clear();

        // 加载游戏，恢复状态
        saveManager.LoadGame(player, quest, world);

        // 输出恢复后的状态验证
        Console.WriteLine($"HP: {player.HP}, MP: {player.MP}, Position: {player.Position}");
        Console.WriteLine($"Inventory: {string.Join(", ", player.Inventory)}");
        Console.WriteLine($"Quests: Find the Ring - {quest.CompletedQuests["Find the Ring"]}");
        Console.WriteLine($"Discovered Areas: {string.Join(", ", world.DiscoveredAreas)}");
    }
}
