using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApplication1.Helper;
using WebApplication1.Service;
using YourNamespace.Hubs;
using static WebApplication1.Dto.UserDto;

var builder = WebApplication.CreateBuilder(args);

// 读取 Jwt 设置
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

// 添加 SignalR 服务
builder.Services.AddSignalR();

// 添加 CORS 策略，允许 Vue 开发服务器跨域请求
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueDevClient", policy =>
    {
        policy.WithOrigins("http://localhost:8080")  // 你的前端地址
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // 支持携带 Cookie 和 Authorization Header
    });
});

// 添加内存缓存
builder.Services.AddMemoryCache();

// 配置 JwtSettings
builder.Services.Configure<JwtSettings>(jwtSettings);

// 配置 JWT 认证
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };

        // 支持 SignalR 通过查询字符串传递 Token
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

// 添加授权服务
builder.Services.AddAuthorization();

// 业务服务注册
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddSingleton<VerificationCodeService>();

// 数据库上下文配置（MySQL）
builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySqlConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySqlConnection"))
    );
});

// 添加 MVC 控制器
builder.Services.AddControllers();

// 添加 Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApplication1 API v1", Version = "v1" });
    c.SwaggerDoc("user", new OpenApiInfo { Title = "用户管理", Version = "user" });
});

var app = builder.Build();

// 开发环境启用 Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplication1 API v1");
        c.SwaggerEndpoint("/swagger/user/swagger.json", "用户管理");
    });
}

// 强制 HTTPS 重定向
app.UseHttpsRedirection();

// **注意顺序：先路由，再跨域，再认证授权**
app.UseRouting();

// 使用 CORS 策略（必须在 UseRouting 之后，UseAuthentication 之前或之后都行，但至少要在 UseEndpoints 之前）
app.UseCors("AllowVueDevClient");

// 认证和授权中间件顺序不能反了
app.UseAuthentication();
app.UseAuthorization();

// 映射 SignalR Hub，路径大小写要和前端完全一致
app.MapHub<ChatHub>("/chatHub");

// 映射 API 控制器
app.MapControllers();

// 启动应用
app.Run();
