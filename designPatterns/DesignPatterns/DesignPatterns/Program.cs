using ChainOfResponsibilityDemo;

class Program
{
    static void Main()
    {
        int incomingDamage = 100;

        // 组装责任链
        var immunity = new ImmunityHandler(isImmune: false);
        var shield = new ShieldHandler(shieldPoints: 30);
        var buff = new DamageReductionBuffHandler(reductionRate: 0.2);
        var armor = new ArmorHandler(armorValue: 10);

        immunity.SetNext(shield);
        shield.SetNext(buff);
        buff.SetNext(armor);

        Console.WriteLine($"初始伤害：{incomingDamage}");
        immunity.Handle(ref incomingDamage);
        Console.WriteLine($"最终承受伤害：{incomingDamage}");
    }
}