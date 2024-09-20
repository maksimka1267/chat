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

            // ���������� ����������� �����
            builder.Services.AddAutoMapper(typeof(DataBaseMappings));
            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication(); // ���������, ����� �� ��� ���� �����
            builder.Services.AddGrpc();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();
            builder.Services.AddGrpcClient<ChatService.ChatServiceClient>(o =>
            {
                o.Address = new Uri("http://localhost:5146");
            });
            // ��������� ���� ������
            builder.Services.AddDbContext<MessageDBContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString(nameof(MessageDBContext)))
            );

            // ���� �� ����������� TempData, �������� ��������� ������
            builder.Services.AddSession(); // ��� ������ � ��������
            builder.Services.AddControllersWithViews()
                .AddSessionStateTempDataProvider(); // ��� TempData

            var app = builder.Build();

            // ������������� ������
            app.UseSession(); // �������� ���, ���� ����������� ������

            app.MapGrpcService<MessageServiceImpl>();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}
