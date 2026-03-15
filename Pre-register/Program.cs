using Pre_register.Data;
using Pre_register.Extensions;
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

// Data - Singleton เพราะใช้ข้อมูลจำลอง  
builder.Services.AddSingleton<AppDbContext>();

// Infrastructure
builder.Services.AddSingleton<TokenGenerator>();
builder.Services.AddSingleton<QRCodeGenerator>();
builder.Services.AddSingleton<LprService>();


builder.Services.RegisterDependencies();
 

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
