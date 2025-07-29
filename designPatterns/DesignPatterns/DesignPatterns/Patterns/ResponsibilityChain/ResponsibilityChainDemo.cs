using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

namespace ChainOfResponsibilityDemo
{
    // 抽象处理者
    public abstract class DamageHandler
    {
        protected DamageHandler NextHandler;

        public void SetNext(DamageHandler next)
        {
            NextHandler = next;
        }

        public void Handle(ref int damage)
        {
            // 当前处理者处理伤害
            if (!ProcessDamage(ref damage))
            {
                // 如果当前处理者没完全处理，传递给下一个处理者
                NextHandler?.Handle(ref damage);
            }
            else
            {
                // 处理完毕
                Console.WriteLine("伤害处理完毕。");
            }
        }

        // 返回true表示完全处理，不再往后传递
        protected abstract bool ProcessDamage(ref int damage);
    }

    // 护甲处理者
    public class ArmorHandler : DamageHandler
    {
        private readonly int armorValue;

        public ArmorHandler(int armorValue)
        {
            this.armorValue = armorValue;
        }

        protected override bool ProcessDamage(ref int damage)
        {
            Console.WriteLine($"护甲吸收 {armorValue} 点伤害。");
            damage -= armorValue;
            if (damage <= 0)
            {
                damage = 0;
                Console.WriteLine("伤害被护甲完全吸收。");
                return true; // 完全处理，不传递了
            }
            return false; // 还没处理完，继续传递
        }
    }

    // 减伤Buff处理者
    public class DamageReductionBuffHandler : DamageHandler
    {
        private readonly double reductionRate; // 比如0.2代表减伤20%

        public DamageReductionBuffHandler(double reductionRate)
        {
            this.reductionRate = reductionRate;
        }

        protected override bool ProcessDamage(ref int damage)
        {
            int reducedAmount = (int)(damage * reductionRate);
            Console.WriteLine($"减伤Buff减少 {reducedAmount} 点伤害。");
            damage -= reducedAmount;
            if (damage <= 0)
            {
                damage = 0;
                Console.WriteLine("伤害被减伤Buff完全抵消。");
                return true;
            }
            return false;
        }
    }

    // 护盾处理者
    public class ShieldHandler : DamageHandler
    {
        private int shieldPoints;

        public ShieldHandler(int shieldPoints)
        {
            this.shieldPoints = shieldPoints;
        }

        protected override bool ProcessDamage(ref int damage)
        {
            if (shieldPoints <= 0)
            {
                Console.WriteLine("护盾已破碎，无伤害吸收。");
                return false;
            }

            int absorbed = Math.Min(damage, shieldPoints);
            shieldPoints -= absorbed;
            damage -= absorbed;
            Console.WriteLine($"护盾吸收了 {absorbed} 点伤害，剩余护盾：{shieldPoints}。");

            if (damage <= 0)
            {
                damage = 0;
                Console.WriteLine("伤害被护盾完全吸收。");
                return true;
            }
            return false;
        }
    }

    // 免伤状态处理者
    public class ImmunityHandler : DamageHandler
    {
        private readonly bool isImmune;

        public ImmunityHandler(bool isImmune)
        {
            this.isImmune = isImmune;
        }

        protected override bool ProcessDamage(ref int damage)
        {
            if (isImmune)
            {
                Console.WriteLine("角色处于免伤状态，伤害为0。");
                damage = 0;
                return true;
            }
            return false;
        }
    }

    //class Program
    //{
    //    static void Main()
    //    {
    //        int incomingDamage = 100;

    //        // 组装责任链
    //        var immunity = new ImmunityHandler(isImmune: false);
    //        var shield = new ShieldHandler(shieldPoints: 30);
    //        var buff = new DamageReductionBuffHandler(reductionRate: 0.2);
    //        var armor = new ArmorHandler(armorValue: 10);

    //        immunity.SetNext(shield);
    //        shield.SetNext(buff);
    //        buff.SetNext(armor);

    //        Console.WriteLine($"初始伤害：{incomingDamage}");
    //        immunity.Handle(ref incomingDamage);
    //        Console.WriteLine($"最终承受伤害：{incomingDamage}");
    //    }
    //}
}

