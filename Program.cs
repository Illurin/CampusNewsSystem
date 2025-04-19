using CampusNewsSystem.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 添加 Razor Pages 和 MVC 控制器支持
builder.Services.AddControllersWithViews();

// 配置 SQLite 数据库
builder.Services.AddDbContext<AppDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 添加 Cookie 认证
builder.Services.AddAuthentication("CookieAuth")
	.AddCookie("CookieAuth", config =>
	{
		config.LoginPath = "/Auth/Login";
		config.AccessDeniedPath = "/Auth/AccessDenied";
		config.ExpireTimeSpan = TimeSpan.FromHours(2);
	});

builder.Services.AddAuthorization();

var app = builder.Build();

// 开发环境调试页面
if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}
else
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

// 中间件配置顺序
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // 认证中间件
app.UseAuthorization();  // 授权中间件

// 默认路由：打开程序即跳到登录页
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();