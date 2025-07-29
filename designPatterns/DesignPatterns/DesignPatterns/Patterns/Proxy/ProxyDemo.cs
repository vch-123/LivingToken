using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//接下来就轮到结构型的 代理模式（Proxy Pattern） 了：

//✅ 它解决的不是“扩展功能”，而是“控制访问”，比如：
//惰性加载（Lazy loading）

//权限控制（如登录拦截）

//远程代理（远程方法调用）

//缓存代理（缓存结果防止重复计算）

//我马上给你准备一个真实感强的 C# 示例，
//模拟一个用户服务，只有管理员能删除用户，来体现“控制访问”。

//稍等我一下，我给你码出来 🧑‍💻。


//接口 共同抽象
public interface IUserService
{
    void DeleteUser(string username);
}

//真实干活
public class RealUserService : IUserService
{
    public void DeleteUser(string username)
    {
        Console.WriteLine($"✅ 用户 {username} 已被删除！");
    }
}

//代理类，加权限控制
public class UserServiceProxy : IUserService
{
    private readonly RealUserService _realService;
    private readonly string _currentUserRole;

    public UserServiceProxy(string currentUserRole)
    {
        _realService = new RealUserService();
        _currentUserRole = currentUserRole;
    }

    public void DeleteUser(string username)
    {
        if (_currentUserRole != "Admin")
        {
            Console.WriteLine("❌ 没有权限删除用户！");
            return;
        }

        _realService.DeleteUser(username);
    }
}



//class Program
//{
//    static void Main()
//    {
//        Console.WriteLine("---- 普通用户尝试删除 ----");
//        IUserService normalUserService = new UserServiceProxy("User");
//        normalUserService.DeleteUser("Tom");

//        Console.WriteLine("\n---- 管理员尝试删除 ----");
//        IUserService adminUserService = new UserServiceProxy("Admin");
//        adminUserService.DeleteUser("Jerry");
//    }
//}
