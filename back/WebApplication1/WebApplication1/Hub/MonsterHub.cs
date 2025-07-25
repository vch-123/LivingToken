using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MonsterHub : Hub
{
    private class Monster
    {
        public string ConnectionId { get; set; }
        public string UserName { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Rotation { get; set; } = 0;
    }

    private class Bullet
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Direction { get; set; } // 0-359 degrees
    }

    // 线程安全存储所有怪兽，Key用用户名唯一标识
    private static ConcurrentDictionary<string, Monster> Monsters = new ConcurrentDictionary<string, Monster>();

    // 所有子弹
    private static List<Bullet> Bullets = new List<Bullet>();

    private static Random rand = new Random();

    // 位置限制
    private const int MaxX = 600;
    private const int MaxY = 400;

    // 玩家连接时，发送自己id
    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("YourId", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        // 玩家断开，移除怪兽
        var toRemove = new List<string>();
        foreach (var kvp in Monsters)
        {
            if (kvp.Value.ConnectionId == Context.ConnectionId)
            {
                toRemove.Add(kvp.Key);
            }
        }
        foreach (var user in toRemove)
        {
            Monsters.TryRemove(user, out _);
        }
        await BroadcastState();
        await base.OnDisconnectedAsync(exception);
    }

    // 注册玩家怪兽，只注册一次（用户名唯一）
    public async Task Register(string userName)
    {
        if (string.IsNullOrEmpty(userName)) userName = "匿名";

        if (!Monsters.ContainsKey(userName))
        {
            var monster = new Monster()
            {
                UserName = userName,
                ConnectionId = Context.ConnectionId,
                X = rand.Next(0, MaxX - 40),
                Y = rand.Next(0, MaxY - 40),
                Rotation = 0
            };
            Monsters[userName] = monster;
        }
        else
        {
            // 更新连接ID（防止重连变成多个）
            Monsters[userName].ConnectionId = Context.ConnectionId;
        }

        await BroadcastState();
    }

    public async Task Move(int deltaX, int deltaY)
    {
        var monster = FindMonsterByConnection(Context.ConnectionId);
        if (monster == null) return;

        monster.X = Math.Clamp(monster.X + deltaX, 0, MaxX - 40);
        monster.Y = Math.Clamp(monster.Y + deltaY, 0, MaxY - 40);

        await BroadcastState();
    }

    public async Task Rotate()
    {
        var monster = FindMonsterByConnection(Context.ConnectionId);
        if (monster == null) return;

        monster.Rotation = (monster.Rotation + 360) % 360; // 旋转一周
        await BroadcastState();
    }

    public async Task Shoot()
    {
        var monster = FindMonsterByConnection(Context.ConnectionId);
        if (monster == null) return;

        // 子弹从怪兽中心发射，方向朝怪兽旋转角度
        var bullet = new Bullet()
        {
            X = monster.X + 20, // 中心点偏移
            Y = monster.Y + 20,
            Direction = monster.Rotation
        };

        lock (Bullets)
        {
            Bullets.Add(bullet);
            if (Bullets.Count > 100) Bullets.RemoveAt(0); // 限制子弹数量
        }

        await BroadcastState();
    }

    // 定时更新子弹位置（前端只收最新状态）
    // 这里简化每次操作都更新，或你可用定时器做后端推送
    private void UpdateBullets()
    {
        lock (Bullets)
        {
            for (int i = Bullets.Count - 1; i >= 0; i--)
            {
                var b = Bullets[i];
                // 以方向为角度移动，速度10像素每次
                double rad = b.Direction * Math.PI / 180.0;
                b.X += (int)(10 * Math.Cos(rad));
                b.Y += (int)(10 * Math.Sin(rad));

                // 超出边界移除
                if (b.X < 0 || b.X > MaxX || b.Y < 0 || b.Y > MaxY)
                {
                    Bullets.RemoveAt(i);
                }
            }
        }
    }

    private Monster FindMonsterByConnection(string connectionId)
    {
        foreach (var kvp in Monsters)
        {
            if (kvp.Value.ConnectionId == connectionId) return kvp.Value;
        }
        return null;
    }

    private async Task BroadcastState()
    {
        UpdateBullets();

        // 转换数据成简单字典方便序列化
        var monsterData = new Dictionary<string, object>();
        foreach (var kvp in Monsters)
        {
            monsterData[kvp.Key] = new
            {
                x = kvp.Value.X,
                y = kvp.Value.Y,
                rotation = kvp.Value.Rotation,
                name = kvp.Value.UserName
            };
        }

        var bulletData = new List<object>();
        lock (Bullets)
        {
            foreach (var b in Bullets)
            {
                bulletData.Add(new { x = b.X, y = b.Y });
            }
        }

        await Clients.All.SendAsync("UpdateMonsters", monsterData);
        await Clients.All.SendAsync("UpdateBullets", bulletData);
    }
}
