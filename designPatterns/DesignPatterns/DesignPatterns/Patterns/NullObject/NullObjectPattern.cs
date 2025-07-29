using NullObjectPatternDemo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NullObjectPatternDemo
{
    // 武器接口
    public interface IWeapon
    {
        void Attack();
    }

    // 具体武器
    public class Sword : IWeapon
    {
        public void Attack()
        {
            Console.WriteLine("挥舞剑进行攻击！");
        }
    }

    // 空对象武器（空实现）
    public class NullWeapon : IWeapon
    {
        public void Attack()
        {
            // 什么都不做，空实现
        }
    }

    // 玩家类
    public class Player
    {
        private IWeapon _weapon;

        public Player(IWeapon weapon)
        {
            // 如果传入null，用空对象代替
            _weapon = weapon ?? new NullWeapon();
        }

        public void Attack()
        {
            _weapon.Attack();
        }
    }
}

//class Program
//{
//    static void Main()
//    {
//        IWeapon sword = new Sword();
//        Player playerWithSword = new Player(sword);
//        playerWithSword.Attack(); // 输出: 挥舞剑进行攻击！

//        Player playerWithNoWeapon = new Player(null);
//        playerWithNoWeapon.Attack(); // 什么都不输出，也不会报错
//    }
//}
