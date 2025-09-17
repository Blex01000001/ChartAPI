using ChartAPI.Hubs;
using ChartAPI.Interfaces;
using ChartAPI.Repositories;
using ChartAPI.Services;
using Microsoft.AspNetCore.SignalR;
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


// ���U CORS �A�ȡA�o�̥��w�q�@�ӥ��}�� Policy
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll", policy =>
//    {
//        policy
//            .AllowAnyOrigin()    // ���\�Ҧ��ӷ�
//            .AllowAnyMethod()    // ���\�Ҧ� HTTP ��k�]GET�BPOST ���^
//            .AllowAnyHeader();   // ���\�Ҧ����Y
//    });
//});
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowFrontend", policy =>
//    {
//        policy.WithOrigins("file:///D:/Program/CS/ChartAPI/wwwroot/DeptYearChar.html") // �o�̧令�A���e�ݹ�ں��}
//              .AllowAnyHeader()
//              .AllowAnyMethod()
//              .AllowCredentials();
//    });
//});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.WebHost.UseUrls("http://localhost:5265"); // �j����w URL
// ���U SignalR
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();// ��� Swagger �e��
    app.UseSwaggerUI();// ��� Swagger �e��
}
//app.UseCors("AllowAll");
app.UseCors("AllowFrontend");

app.MapHub<NotifyHub>("/notifyHub");// �M�g Hub
//app.UseStaticFiles();// �[�J�o��G�ҥ��R�A�ɮתA��
//app.MapFallbackToFile("index.html");// �i��G�w�]�ɦV index.html�]�p�G�O SPA �i�Ρ^

app.UseAuthorization();// �[�J���v�]�����p�O�d�^

app.MapControllers();// �[�J API ����

app.Run();
