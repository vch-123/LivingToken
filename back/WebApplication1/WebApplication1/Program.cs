using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebApplication1.Service;
using Pomelo.EntityFrameworkCore.MySql;
using WebApplication1.Helper;

var builder = WebApplication.CreateBuilder(args);

// 添加 CORS 配置
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
builder.Services.AddMemoryCache();   // .NET 8 内置
// Add services to the container.

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<VerificationCodeService>();

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("MySqlConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySqlConnection")));
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // 启用分组支持
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApplication1 API v1", Version = "v1" });
    c.SwaggerDoc("user", new OpenApiInfo { Title = "用户管理", Version = "user" });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplication1 API v1");
        c.SwaggerEndpoint("/swagger/user/swagger.json", "用户管理");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
string password = "1";
Stopwatch stopwatch = new Stopwatch();
stopwatch.Start();
string passwordhash = PasswordHelper.GeneratePassword(password);
Console.WriteLine(passwordhash);
stopwatch.Stop();
Console.WriteLine($"Method execution time: {stopwatch.ElapsedMilliseconds} ms");
// 启用 CORS 中间件
app.UseCors("AllowAllOrigins");
app.Run();

