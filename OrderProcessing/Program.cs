using OrderProcessing.Data;
using OrderProcessing.Extensions;
using OrderProcessing.Infrastructure;
using OrderProcessing.Integrations;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Redis
var redisConnection = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnection));

// Infrastructure (Singleton)
builder.Services.AddSingleton<AppDbContext>();
builder.Services.AddSingleton<RedisCacheService>();
builder.Services.AddSingleton<DistributedLockService>();
builder.Services.AddSingleton<IdempotencyService>();
builder.Services.AddSingleton<PaymentGatewayClient>();

// Services & Repositories (Scoped - auto-registered)
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
