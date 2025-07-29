using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class AbstractClass
{
    // 模板方法，固定流程，防止子类重写
    public void TemplateMethod()
    {
        Step1();
        Hook1();       // 钩子
        Step2();
        Hook2();       // 钩子
        Step3();
    }


    protected abstract void Step1();
    protected abstract void Step2();
    protected abstract void Step3();

    // 钩子方法，默认空实现，子类可选重写
    protected virtual void Hook1() { }
    protected virtual void Hook2() { }
}


public class ConcreteClass : AbstractClass
{
    protected override void Step1() => Console.WriteLine("步骤1");
    protected override void Step2() => Console.WriteLine("步骤2");
    protected override void Step3() => Console.WriteLine("步骤3");

    protected override void Hook1() => Console.WriteLine("重写钩子1");
}


//var obj = new ConcreteClass();
//obj.TemplateMethod();
