using Amg.Authentication.Host.SeedWorks;
using Amg.Authentication.Infrastructure.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Amg.Authentication.Application.Contract;
using Amg.Authentication.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Amg.Authentication.DomainModel.Modules.Roles;
using Amg.Authentication.DomainModel.Modules.Users;
using Amg.Authentication.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using Amg.Authentication.Host.Filters;
using Microsoft.AspNetCore.Identity;
using Amg.Authentication.Host.Services;
using Microsoft.Extensions.Hosting;
using MassTransit;
using Amg.Authentication.EventHandler.Modules.UserActivities;

namespace Amg.Authentication.Host.ProgramSetup
{
    public static class ProgramHelper
    {
     
        public static void SetupAppSettingsConfig(WebApplicationBuilder builder)
        {
            builder.Services.Configure<HostSettings>(builder.Configuration.GetSection(nameof(HostSettings)));
            builder.Services.Configure<CaptchaSettings>(builder.Configuration.GetSection(nameof(CaptchaSettings)));
            builder.Services.Configure<NotificationSettings>(builder.Configuration.GetSection(nameof(NotificationSettings)));
            builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection(nameof(AuthSettings)));
            builder.Services.Configure<JwtTokenSettings>(builder.Configuration.GetSection(nameof(JwtTokenSettings)));
            builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection(nameof(RabbitMqSettings)));
        }

        public static void ConfigureControllers(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<DefaultExceptionFilter>();
            })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            builder.Services.AddSingleton(provider => provider
                .GetService<IOptions<JsonOptions>>().Value.JsonSerializerOptions);

            builder.Services.AddCors(options => options.AddPolicy(Constants.DefaultCorsPolicy, builder =>
            {
                // a.ammari : cannot use AllowAnyOrigin() because Allowing any credentials
                builder.SetIsOriginAllowed(_ => true)
                     .AllowAnyMethod()
                     .AllowAnyHeader()
                     .AllowCredentials();

            }));

            builder.Services.AddHttpContextAccessor();
        }

        public static void ConfigureDatabases(WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseConnection"));
            });

            builder.Services.AddDbContext<IdentityContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseConnection"));
            });
        }

        public static void ConfigureAuthentication(WebApplicationBuilder builder)
        {
            var authSettings = builder.Configuration.GetSection(nameof(AuthSettings)).Get<AuthSettings>();
            var jwtTokenSettings = builder.Configuration.GetSection(nameof(JwtTokenSettings)).Get<JwtTokenSettings>();

            builder.Services.AddScoped<PermissionAuthorizationFilter>();

            builder.Services.AddIdentity<User, Role>(options =>
            {
                // Lockout settings
                options.Lockout.AllowedForNewUsers = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(authSettings.Lockout.LockTime);
                options.Lockout.MaxFailedAccessAttempts = authSettings.Lockout.Threshold;
                // Password settings
                options.Password.RequireDigit = authSettings.Password.RequireDigits;
                options.Password.RequireLowercase = authSettings.Password.RequireLowercase;
                options.Password.RequireUppercase = authSettings.Password.RequireUppercase;
                options.Password.RequireNonAlphanumeric = authSettings.Password.RequireSymbols;
                options.Password.RequiredLength = authSettings.Password.MinimumLength;
                options.Password.RequiredUniqueChars = authSettings.Password.MinimumUniqueChars;
                // Claims settings
                options.ClaimsIdentity.RoleClaimType = AuthConstants.UserRolesClaimType;
                options.ClaimsIdentity.UserIdClaimType = AuthConstants.UserIdClaimType;
                options.ClaimsIdentity.UserNameClaimType = AuthConstants.UserNameClaimType;
                // signIn settings
                options.SignIn.RequireConfirmedAccount = true;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = true;
                // User settings
                options.User.RequireUniqueEmail = false;
                // Stores settings
                options.Stores.ProtectPersonalData = false;
            }).AddEntityFrameworkStores<IdentityContext>()
                .AddErrorDescriber<PersianIdentityErrorDescriber>()
                .AddDefaultTokenProviders();


            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.ClaimsIssuer = jwtTokenSettings.Issuer;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtTokenSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtTokenSettings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenSettings.ValidationKey)),
                    TokenDecryptionKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenSettings.DecryptionKey)),
                    ValidateLifetime = true,
                    NameClaimType = AuthConstants.UserNameClaimType,
                    RoleClaimType = AuthConstants.UserRolesClaimType
                };
                options.SaveToken = true;
               /* options.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.HttpContext.Request.Cookies[AuthConstants.CookieName]                
                        // a.ammari: DO NOT SET context.Result.
                        return Task.CompletedTask;
                    }
                };*/
            }).AddGoogle(options =>
            {
                IConfigurationSection googleAuthNSection =
                    builder.Configuration.GetSection("Authentication:Google");

                string clientID = googleAuthNSection["ClientId"];
                string clientSecret = googleAuthNSection["ClientSecret"];

                string envClientId = Environment.GetEnvironmentVariable("Authentication_Google_ClientId");
                string envClientSecret = Environment.GetEnvironmentVariable("Authentication_Google_ClientSecret");

                options.ClientId = !string.IsNullOrEmpty(envClientId) ? envClientId : clientID;
                options.ClientSecret = !string.IsNullOrEmpty(envClientSecret) ? envClientSecret : clientSecret;
            });

            builder.Services.AddAuthorization();
        }

        public static void ConfigureMassTransit(WebApplicationBuilder builder)
        {
            var rabbitMqSettings = builder.Configuration.GetSection(nameof(RabbitMqSettings)).Get<RabbitMqSettings>();

            builder.Services.AddMassTransit(c =>
            {
                c.AddConsumers(typeof(UserActivityEventHandler).Assembly);

                c.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host(new Uri(rabbitMqSettings.Address), h =>
                    {
                        h.Username(rabbitMqSettings.Username);
                        h.Password(rabbitMqSettings.Password);
                    });

                    cfg.ReceiveEndpoint(rabbitMqSettings.QueueName, e =>
                    {
                        e.PrefetchCount = 16;
                        e.UseMessageRetry(x => x.Interval(2, 100));

                        e.ConfigureConsumers(provider);
                    });

                    // or, configure the endpoints by convention
                    //cfg.ConfigureEndpoints(provider);
                }));

            });

            builder.Services.AddSingleton<IHostedService, BusService>();
        }
    }
}
