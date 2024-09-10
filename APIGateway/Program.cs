using APIGateway.Interface;
using APIGateway.Repository;
using ClientService;
using Microsoft.AspNetCore.CookiePolicy;
using ServiceChat;
using ServiceMessage;
namespace APIGateway;
public class Program
{
    public static void Main(string[] args)
    {
        var builder=WebApplication.CreateBuilder(args);
        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddAuthorization();
        builder.Services.AddAutoMapper(typeof(MappingProfile));
        builder.Services.AddScoped<IClientRepository, ClientRepository>();
        builder.Services.AddScoped<IMessageRepository, MessageRepository>();
        builder.Services.AddScoped<IChatRepository, ChatRepository>();
        builder.Services.AddGrpcClient<ClientAccount.ClientAccountClient>(o =>
        {
            o.Address = new Uri("https://localhost:7048");
        });
        builder.Services.AddGrpcClient<MessageService.MessageServiceClient>(o =>
        {
            o.Address = new Uri("http://localhost:5067");
        });
        builder.Services.AddGrpcClient<ChatService.ChatServiceClient>(o =>
        {
            o.Address = new Uri("http://localhost:5146");
        });
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("Content-Disposition"));
        });

        var app = builder.Build();

        app.UseCookiePolicy(new CookiePolicyOptions
        {
            MinimumSameSitePolicy = SameSiteMode.Strict,
            HttpOnly = HttpOnlyPolicy.Always,
            Secure = CookieSecurePolicy.Always
        });

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseHttpsRedirection();

        app.UseCors("CorsPolicy");

        app.UseAuthorization();

        app.MapControllers();


        app.Run();
    }
}