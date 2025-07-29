using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;

//如果你碰到的业务场景中存在“对象行为随状态变化”或者“复杂状态切换”，就非常适合用状态模式。

public interface ICharacterState
{
    void HandleInput(Character context, string input);
    void Update(Character context);
    string GetStateName();
}

public class RestState : ICharacterState
{
    public void HandleInput(Character context, string input)
    {
        switch (input)
        {
            case "walk":
                Console.WriteLine("从休息转为慢走");
                context.SetState(new WalkState());
                break;
            case "run":
                Console.WriteLine("从休息转为快跑");
                context.SetState(new RunState());
                break;
            case "crouch":
                Console.WriteLine("从休息转为蹲下");
                context.SetState(new CrouchState());
                break;
            case "jump":
                Console.WriteLine("从休息转为跳跃");
                context.SetState(new JumpState());
                break;
            default:
                Console.WriteLine("休息中...");
                break;
        }
    }

    public void Update(Character context)
    {
        Console.WriteLine("角色正在休息...");
    }

    public string GetStateName() => "休息";
}

public class WalkState : ICharacterState
{
    public void HandleInput(Character context, string input)
    {
        switch (input)
        {
            case "run":
                Console.WriteLine("从慢走转为快跑");
                context.SetState(new RunState());
                break;
            case "rest":
                Console.WriteLine("从慢走转为休息");
                context.SetState(new RestState());
                break;
            case "crouch":
                Console.WriteLine("从慢走转为蹲下");
                context.SetState(new CrouchState());
                break;
            case "jump":
                Console.WriteLine("从慢走转为跳跃");
                context.SetState(new JumpState());
                break;
            default:
                Console.WriteLine("慢走中...");
                break;
        }
    }

    public void Update(Character context)
    {
        Console.WriteLine("角色正在慢走...");
    }

    public string GetStateName() => "慢走";
}

public class RunState : ICharacterState
{
    public void HandleInput(Character context, string input)
    {
        switch (input)
        {
            case "walk":
                Console.WriteLine("从快跑转为慢走");
                context.SetState(new WalkState());
                break;
            case "rest":
                Console.WriteLine("从快跑转为休息");
                context.SetState(new RestState());
                break;
            case "crouch":
                Console.WriteLine("从快跑转为蹲下");
                context.SetState(new CrouchState());
                break;
            case "jump":
                Console.WriteLine("从快跑转为跳跃");
                context.SetState(new JumpState());
                break;
            default:
                Console.WriteLine("快跑中...");
                break;
        }
    }

    public void Update(Character context)
    {
        Console.WriteLine("角色正在快跑...");
    }

    public string GetStateName() => "快跑";
}

public class CrouchState : ICharacterState
{
    public void HandleInput(Character context, string input)
    {
        switch (input)
        {
            case "walk":
                Console.WriteLine("从蹲下转为慢走");
                context.SetState(new WalkState());
                break;
            case "rest":
                Console.WriteLine("从蹲下转为休息");
                context.SetState(new RestState());
                break;
            case "run":
                Console.WriteLine("从蹲下转为快跑");
                context.SetState(new RunState());
                break;
            case "jump":
                Console.WriteLine("从蹲下转为跳跃");
                context.SetState(new JumpState());
                break;
            default:
                Console.WriteLine("蹲下中...");
                break;
        }
    }

    public void Update(Character context)
    {
        Console.WriteLine("角色正在蹲下...");
    }

    public string GetStateName() => "蹲下";
}

public class JumpState : ICharacterState
{
    private int jumpCount = 0;

    public void HandleInput(Character context, string input)
    {
        Console.WriteLine("跳跃中，不能切换状态");
    }

    public void Update(Character context)
    {
        Console.WriteLine("角色正在跳跃...");
        jumpCount++;

        if (jumpCount >= 3)
        {
            Console.WriteLine("跳跃结束，回到休息状态");
            context.SetState(new RestState());
        }
    }

    public string GetStateName() => "跳跃";
}

public class Character
{
    private ICharacterState _state;

    public Character()
    {
        _state = new RestState();
    }

    public void SetState(ICharacterState newState)
    {
        _state = newState;
        Console.WriteLine($"状态切换到：{_state.GetStateName()}");
    }

    public void HandleInput(string input)
    {
        _state.HandleInput(this, input);
    }

    public void Update()
    {
        _state.Update(this);
    }
}



//class Program
//{
//    static void Main()
//    {
//        var character = new Character();

//        character.Update();
//        character.HandleInput("walk");
//        character.Update();

//        character.HandleInput("run");
//        character.Update();

//        character.HandleInput("jump");
//        character.Update();
//        character.Update();
//        character.Update();

//        character.HandleInput("crouch");
//        character.Update();

//        character.HandleInput("rest");
//        character.Update();
//    }
//}