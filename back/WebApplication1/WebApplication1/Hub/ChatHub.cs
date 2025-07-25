using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;

namespace YourNamespace.Hubs
{
    [Authorize] // 可选：要求连接时就验证
    public class ChatHub : Hub
    {
        public async Task SendMessage(string message)
        {
            var user = Context.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                await Clients.Caller.SendAsync("ReceiveSystemMessage", "请先登录才能发送消息！");
                return;
            }

            var username = user.Identity.Name ?? "匿名"; // 从 token 中提取的 Name
            await Clients.All.SendAsync("ReceiveMessage", username, message);
        }
    }
}
