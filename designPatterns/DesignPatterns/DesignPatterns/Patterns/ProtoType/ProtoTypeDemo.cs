using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 原型接口
public interface IPrototype<T>
{
    T Clone();
}

// 具体类
public class Enemy : IPrototype<Enemy>
{
    public string Name { get; set; }
    public Weapon Weapon { get; set; }

    // 浅拷贝
    public Enemy Clone()
    {
        return (Enemy)this.MemberwiseClone();
    }
    public Enemy DeepClone()
    {
        return new Enemy
        {
            Name = this.Name,
            Weapon = new Weapon { Name = this.Weapon.Name }
        };
    }

}

// 引用类型
public class Weapon
{
    public string Name { get; set; }
}



//var original = new Enemy
//{
//    Name = "Orc",
//    Weapon = new Weapon { Name = "Axe" }
//};

//var clone = original.Clone();

//clone.Name = "Troll";
//clone.Weapon.Name = "Sword";

//Console.WriteLine(original.Name);  // Orc
//Console.WriteLine(original.Weapon.Name); // Sword ❗（浅拷贝问题）


//🏗 五、适用场景
//场景	说明
//对象创建成本高	如：数据库查询、大量配置
//对象构建逻辑复杂	避免重复构建流程
//对象种类繁多	使用已有对象模板快速生成
//游戏对象复用	如角色、敌人、子弹等克隆

//🧪 六、原型模式 vs new
//对比项       new创建       原型模式
//性能         每次全新构建	直接复制，性能高
//代码复用	  差	           好（可以从模板快速构建）
//复杂对象	  繁琐	        简单
//拷贝控制	  不可控	        支持深/浅拷贝