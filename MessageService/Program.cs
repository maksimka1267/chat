using MessageService;
using Microsoft.EntityFrameworkCore;
using MessageService.Data;
using MessageService.Repository;
using MessageService.Services;
namespace MessageService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            builder.Services.AddAutoMapper(typeof(DataBaseMappings));
            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication();
            builder.Services.AddGrpc();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();
            builder.Services.AddDbContext<MessageDBContext>(
                options =>
                {
                    options.UseNpgsql(configuration.GetConnectionString(nameof(MessageDBContext)));
                }
                );
            var app = builder.Build();

            app.MapGrpcService<MessageServiceImpl>();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}