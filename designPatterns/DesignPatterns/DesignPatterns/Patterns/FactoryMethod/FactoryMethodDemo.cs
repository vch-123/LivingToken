using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns.FactoryMethod;

public interface IEnemy
{
    void Attack();
}

public class Goblin : IEnemy
{
    public void Attack()
    {
        Console.WriteLine("Goblin attacks with a dagger!");
    }
}

public class Troll : IEnemy
{
    public void Attack()
    {
        Console.WriteLine("Troll smashes with a club!");
    }
}

public interface IEnemyFactory
{
    IEnemy CreateEnemy();
}

public class GoblinFactory : IEnemyFactory
{
    public IEnemy CreateEnemy()
    {
        return new Goblin();
    }
}

public class TrollFactory : IEnemyFactory
{
    public IEnemy CreateEnemy()
    {
        return new Troll();
    }
}


//using Patterns.FactoryMethod;

//Console.WriteLine("== 工厂方法模式 Demo ==");

//// 创建 Goblin
//IEnemyFactory goblinFactory = new GoblinFactory();
//IEnemy goblin = goblinFactory.CreateEnemy();
//goblin.Attack();

//// 创建 Troll
//IEnemyFactory trollFactory = new TrollFactory();
//IEnemy troll = trollFactory.CreateEnemy();
//troll.Attack();
