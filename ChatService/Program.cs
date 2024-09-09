using ChatService;
using ChatService.Data;
using ChatService.Repository;
using ChatService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddAutoMapper(typeof(DataBaseMappings));
builder.Services.AddScoped<IChatRepository, ChatRepository>();
var configuration = builder.Configuration;

builder.Services.AddDbContext<ChatDbContext>(
    options =>
    {
        options.UseNpgsql(configuration.GetConnectionString(nameof(ChatDbContext)));
    }
    );
var app = builder.Build();
app.MapGrpcService<ChatServiceImpl>();
// Configure the HTTP request pipeline.
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
