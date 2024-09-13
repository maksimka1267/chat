using ClientService.Data;
using ClientService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using ClientService.JWT;
using ClientService.Auth;
using ClientService.Repository;

namespace ClientService
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
            builder.Services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
            builder.Services.AddGrpc();
            builder.Services.AddScoped<IJwtProvider, JwtProvider>();
            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<IClientRepository, ClientRepository>();
            builder.Services.AddDbContext<ClientDbContext>(
                options =>
                {
                    options.UseNpgsql(configuration.GetConnectionString(nameof(ClientDbContext)));
                }
                );
            var app = builder.Build();

            app.MapGrpcService<ClientServiceImpl>();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}