using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// 添加 SignalR 服务
builder.Services.AddSignalR();

// 必须添加 CORS 服务，否则 UseCors 会报错
builder.Services.AddCors();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(7247); // 监听所有网卡的 7247 端口
});

var app = builder.Build();

// 使用 CORS 中间件，配置允许跨域访问
app.UseCors(cors =>
    cors
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .SetIsOriginAllowed(_ => true)
);

app.MapHub<MonsterHub>("/monsterHub");

app.Run();

public class MonsterHub : Hub
{
    public async Task Move(int deltaX, int deltaY)
    {
        await Clients.All.SendAsync("UpdatePosition", deltaX, deltaY);
    }
}
