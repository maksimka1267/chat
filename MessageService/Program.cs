using Microsoft.EntityFrameworkCore;
using MessageService.Data;
using MessageService.Repository;
using MessageService.Services;
using ServiceChat;

namespace MessageService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            // Добавление необходимых служб
            builder.Services.AddAutoMapper(typeof(DataBaseMappings));
            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication(); // Проверьте, нужен ли вам этот вызов
            builder.Services.AddGrpc();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();
            builder.Services.AddGrpcClient<ChatService.ChatServiceClient>(o =>
            {
                o.Address = new Uri("http://localhost:5146");
            });
            // Настройка базы данных
            builder.Services.AddDbContext<MessageDBContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString(nameof(MessageDBContext)))
            );

            // Если вы используете TempData, добавьте следующие строки
            builder.Services.AddSession(); // Для работы с сессиями
            builder.Services.AddControllersWithViews()
                .AddSessionStateTempDataProvider(); // Для TempData

            var app = builder.Build();

            // Использование сессий
            app.UseSession(); // Добавьте это, если используете сессии

            app.MapGrpcService<MessageServiceImpl>();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}
