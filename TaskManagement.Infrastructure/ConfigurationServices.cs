using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Domain.Identity;
using TaskManagement.Domain.Respiratory;
using TaskManagement.Domain.Shared.Helpers;
using TaskManagement.Domain.UnitOfWork;
using TaskManagement.Infrastructure.Context;
using TaskManagement.Infrastructure.Repositories;

namespace TaskManagement.Infrastructure
{
    public static class ConfigurationServices
    {
        public static void InfrastructureServices(this IServiceCollection service, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("defaultConnection");
            string Issuer = configuration["JWT:Issuer"];
            string Audience = configuration["JWT:Audience"];

            service.AddDbContext<AppDBContext>(options =>
                    options.UseSqlServer(connectionString));

            service.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
                {
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                        ValidIssuers = new string[] { Issuer },
                        ValidAudiences = new string[] { Audience },
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true
                    };
                });
            service.AddDefaultIdentity<ApplicationUser>(config =>
            {
                config.Password = new PasswordOptions
                {
                    RequiredLength = 8,
                    RequireUppercase = false,
                    RequireNonAlphanumeric = false,
                    RequireLowercase = false,
                    RequireDigit = false
                };
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<AppDBContext>();

            service.AddCors(option =>
            {
                option.AddPolicy("CorsPolicy", corsPolicyBuilder => corsPolicyBuilder
                 .AllowAnyMethod().AllowAnyHeader().AllowCredentials().SetIsOriginAllowed(hostName => true));
            });
            service.AddTransient<AppDBContext>();
            service.AddHttpContextAccessor();
            service.Configure<JWT>(configuration.GetSection("JWT"));
            service.AddTransient<UserResolverService>();
            service.AddTransient<AppInitializer>();
            service.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
            service.AddScoped<IUnitOfWork, TaskManagement.Infrastructure.UnitOfWork.UnitOfWork>();
        }
    }
}
