using ChartAPI.Services;
using ChartAPI.Interfaces;
using ChartAPI.Repositories;
using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddScoped<IChartServices, ChartService>();
builder.Services.AddScoped<IDataRepository, SQLiteRepository>();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();// 顯示 Swagger 畫面
    app.UseSwaggerUI();// 顯示 Swagger 畫面
}
app.UseCors("AllowAll");

//app.UseStaticFiles();// 加入這行：啟用靜態檔案服務
//app.MapFallbackToFile("index.html");// 可選：預設導向 index.html（如果是 SPA 可用）

app.UseAuthorization();// 加入授權（視情況保留）

app.MapControllers();// 加入 API 路由

app.Run();
