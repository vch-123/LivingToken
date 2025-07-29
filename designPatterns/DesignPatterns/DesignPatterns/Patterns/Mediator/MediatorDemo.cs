using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 抽象中介者
public interface IChatMediator
{
    void SendMessage(string message, User sender);
    void RegisterUser(User user);
}

// 具体中介者
public class ChatRoom : IChatMediator
{
    private readonly List<User> _users = new();

    public void RegisterUser(User user)
    {
        _users.Add(user);
    }

    public void SendMessage(string message, User sender)
    {
        foreach (var user in _users)
        {
            if (user != sender)
                user.Receive(message);
        }
    }
}

// 抽象同事类
public abstract class User
{
    protected IChatMediator _mediator;
    public string Name { get; }

    protected User(string name, IChatMediator mediator)
    {
        Name = name;
        _mediator = mediator;
    }

    public abstract void Send(string message);
    public abstract void Receive(string message);
}

// 具体同事类
public class ChatUser : User
{
    public ChatUser(string name, IChatMediator mediator) : base(name, mediator) { } //调用父类构造

    public override void Send(string message)
    {
        Console.WriteLine($"{Name} 发送消息：{message}");
        _mediator.SendMessage(message, this);
    }

    public override void Receive(string message)
    {
        Console.WriteLine($"{Name} 收到消息：{message}");
    }
}

//// 示例入口
//class Program
//{
//    static void Main()
//    {
//        var chatRoom = new ChatRoom();

//        var user1 = new ChatUser("Alice", chatRoom);
//        var user2 = new ChatUser("Bob", chatRoom);
//        var user3 = new ChatUser("Charlie", chatRoom);

//        chatRoom.RegisterUser(user1);
//        chatRoom.RegisterUser(user2);
//        chatRoom.RegisterUser(user3);

//        user1.Send("大家好！");
//        user2.Send("Hello Alice！");
//    }
//}

