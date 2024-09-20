using APIGateway.Interface;
using APIGateway.JWT;
using APIGateway.Repository;
using ClientService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ServiceChat;
using ServiceMessage;
using System.Text;

namespace APIGateway;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Добавление служб в контейнер
        builder.Services.AddControllersWithViews()
            .AddSessionStateTempDataProvider(); // Для поддержки TempData

        builder.Services.AddSession(); // Для работы с сессиями

        // Конфигурация Swagger/OpenAPI
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Настройка аутентификации JWT
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JwtOptions:SecretKey"]))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["cookies"];
                        return Task.CompletedTask;
                    }
                };
            });

        builder.Services.AddAuthorization();
        builder.Services.AddAutoMapper(typeof(MappingProfile));

        // Регистрация репозиториев и других служб
        builder.Services.AddScoped<IJwtProvider, JwtProvider>();
        builder.Services.AddScoped<IClientRepository, ClientRepository>();
        builder.Services.AddScoped<IMessageRepository, MessageRepository>();
        builder.Services.AddScoped<IChatRepository, ChatRepository>();

        // Настройка gRPC-клиентов
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

        // Настройка CORS
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

        // Конфигурация HTTP-запросов
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors("CorsPolicy");

        app.UseSession(); // Использование сессий

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
