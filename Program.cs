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

// ���U CORS �A�ȡA�o�̥��w�q�@�ӥ��}�� Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()    // ���\�Ҧ��ӷ�
            .AllowAnyMethod()    // ���\�Ҧ� HTTP ��k�]GET�BPOST ���^
            .AllowAnyHeader();   // ���\�Ҧ����Y
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();// ��� Swagger �e��
    app.UseSwaggerUI();// ��� Swagger �e��
}
app.UseCors("AllowAll");

//app.UseStaticFiles();// �[�J�o��G�ҥ��R�A�ɮתA��
//app.MapFallbackToFile("index.html");// �i��G�w�]�ɦV index.html�]�p�G�O SPA �i�Ρ^

app.UseAuthorization();// �[�J���v�]�����p�O�d�^

app.MapControllers();// �[�J API ����

app.Run();
