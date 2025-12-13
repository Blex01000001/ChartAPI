using ChartAPI.DataAccess.SQLite.Repositories;
using ChartAPI.Hubs;
using ChartAPI.Interfaces;
using ChartAPI.Repositories;
using ChartAPI.Services;
using Microsoft.AspNetCore.SignalR;
using System.Text;
using ChartAPI.DataAccess.Interfaces;
using ChartAPI.Models;
using ChartAPI.Services.Chart;
using ChartAPI.Services.Queries;
using ChartAPI.Services.Upsert;
using ChartAPI.DataAccess.SQLite.Initializer;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddScoped<IAnnualSummaryService, AnnualSummaryService>();
builder.Services.AddScoped<ICalendarSummaryService, CalendarSummaryService>();
builder.Services.AddScoped<IDepartmentSummaryService, DepartmentSummaryService>();
builder.Services.AddScoped<ISumItemQueryService, SumItemQueryService>();
builder.Services.AddScoped<IManHourRepository, ManHourRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IUpsertDataService, UpsertDataService>();
builder.Services.AddScoped<IDataInitializer, DataInitializer>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// 註冊 CORS 服務，這裡先定義一個全開的 Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()    // 允許所有來源
            .AllowAnyMethod()    // 允許所有 HTTP 方法（GET、POST 等）
            .AllowAnyHeader();   // 允許所有標頭
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        //policy.WithOrigins("file:///D:/Program/CS/ChartAPI/wwwroot/DeptYearChar.html") // 這裡改成你的前端實際網址
        policy.WithOrigins("http://127.0.0.1:5500/MonthlyChart.html") // 這裡改成你的前端實際網址
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

#region 強行允許所有來源，但安全性很差
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowFrontend", policy =>
//    {
//        policy.SetIsOriginAllowed(_ => true)
//              .AllowAnyHeader()
//              .AllowAnyMethod()
//              .AllowCredentials();
//    });
//});
#endregion

builder.WebHost.UseUrls("http://localhost:5265"); // 強制指定 URL
// 註冊 SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();// 顯示 Swagger 畫面
    app.UseSwaggerUI();// 顯示 Swagger 畫面
}
app.UseCors("AllowAll");
app.UseCors("AllowFrontend");

app.MapHub<NotifyHub>("/notifyHub");// 映射 Hub
//app.UseStaticFiles();// 加入這行：啟用靜態檔案服務
//app.MapFallbackToFile("index.html");// 可選：預設導向 index.html（如果是 SPA 可用）

app.UseAuthorization();// 加入授權（視情況保留）

app.MapControllers();// 加入 API 路由

app.Run();
