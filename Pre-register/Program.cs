using Pre_register.Data;
using Pre_register.Infrastructure;
using Pre_register.Repositories;
using Pre_register.Repositories.Interfaces;
using Pre_register.Services;
using Pre_register.Services.Interfaces;
using Pre_register.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Data - Singleton เพราะใช้ข้อมูลจำลอง in-memory
builder.Services.AddSingleton<AppDbContext>();

// Infrastructure
builder.Services.AddSingleton<TokenGenerator>();
builder.Services.AddSingleton<QRCodeGenerator>();
builder.Services.AddSingleton<LprService>();

// Repositories
builder.Services.AddSingleton<IVisitorRepository, VisitorRepository>();
builder.Services.AddSingleton<IGateLogRepository, GateLogRepository>();

// Services
builder.Services.AddScoped<IVisitorService, VisitorService>();
builder.Services.AddScoped<IGateService, GateService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
