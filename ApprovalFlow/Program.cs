using ApprovalFlow.Data;
using ApprovalFlow.Extensions;
using ApprovalFlow.Integrations;
using ApprovalFlow.Workflow;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddSingleton<AppDbContext>();
builder.Services.AddSingleton<ApprovalStateMachine>();
builder.Services.AddSingleton<ERPClient>();

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
