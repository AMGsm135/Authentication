using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Dgab.Authentication.Application.Contract;
using Dgab.Authentication.Application.Contract.Services;
using Dgab.Authentication.Application.Services;
using Dgab.Authentication.Configuration;
using Dgab.Authentication.DomainModel.Modules.Roles;
using Dgab.Authentication.DomainModel.Modules.Users;
using Dgab.Authentication.EventHandler.Modules.UserActivities;
using Dgab.Authentication.Host.Filters;
using Dgab.Authentication.Host.SeedWorks;
using Dgab.Authentication.Host.Services;
using Dgab.Authentication.Infrastructure.Settings;
using Dgab.Authentication.Persistence.Contexts;
using Dgab.Authentication.Shared;
using GreenPipes;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Dgab.Authentication.Host
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureControllers(services);
            ConfigureAppSettings(services);
            ConfigureDatabases(services);
            ConfigureAuthentication(services);
            ConfigureMassTransit(services);
            ConfigureAppServices(services);
            ConfigureHttpClients(services);
            ConfigureSwagger(services);
            Bootstrapper.Start(services, Configuration);
        }

        /// <summary>
        /// Done
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureHttpClients(IServiceCollection services)
        {
            services.AddHttpClient(Constants.NotificationHttpClient, (provider, options) =>
            {
                var settings = provider.GetService<IOptions<HostSettings>>();
                options.BaseAddress = new Uri(settings.Value.NotificationAddress);
            });
        }

        //Done
        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerDocument();
        }

        /// <summary>
        /// Done
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureControllers(IServiceCollection services)
        {
            services.AddControllers(options =>
                {
                    options.Filters.Add<DefaultExceptionFilter>();
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddSingleton(provider => provider
                .GetService<IOptions<JsonOptions>>().Value.JsonSerializerOptions);

            services.AddCors(o => o.AddPolicy(Constants.DefaultCorsPolicy, builder =>
            {
                // a.ammari : cannot use AllowAnyOrigin() because Allowing any credentials
                builder.SetIsOriginAllowed(_ => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            }));

            services.AddHttpContextAccessor();
        }

        /// <summary>
        /// Done
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureAppSettings(IServiceCollection services)
        {
            services.Configure<HostSettings>(Configuration.GetSection(nameof(HostSettings)));
            services.Configure<CaptchaSettings>(Configuration.GetSection(nameof(CaptchaSettings)));
            services.Configure<NotificationSettings>(Configuration.GetSection(nameof(NotificationSettings)));
            services.Configure<AuthSettings>(Configuration.GetSection(nameof(AuthSettings)));
            services.Configure<JwtTokenSettings>(Configuration.GetSection(nameof(JwtTokenSettings)));
            services.Configure<RabbitMqSettings>(Configuration.GetSection(nameof(RabbitMqSettings)));

        }

        /// <summary>
        /// Done
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureDatabases(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DatabaseConnection"));
            });

            services.AddDbContext<IdentityContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DatabaseConnection"));
            });
        }

        /// <summary>
        /// Done
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureAuthentication(IServiceCollection services)
        {
            var authSettings = Configuration.GetSection(nameof(AuthSettings)).Get<AuthSettings>();
            var jwtTokenSettings = Configuration.GetSection(nameof(JwtTokenSettings)).Get<JwtTokenSettings>();

            services.AddScoped<PermissionAuthorizationFilter>();

            services.AddIdentity<User, Role>(options =>
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
                //.AddTokenProvider<CustomPhoneNumberTokenProvider<User>>(Constants.CustomPhoneTokenProvider);

            
            services.AddAuthentication(options =>
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
                options.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.HttpContext.Request.Cookies[AuthConstants.CookieName];
                        // a.ammari: DO NOT SET context.Result.
                        return Task.CompletedTask;
                    }
                };
            }).AddGoogle(options =>
            {
                IConfigurationSection googleAuthNSection =
                    Configuration.GetSection("Authentication:Google");

                options.ClientId = googleAuthNSection["ClientId"];
                options.ClientSecret = googleAuthNSection["ClientSecret"];
            });

            services.AddAuthorization();
        }

        /// <summary>
        /// Done
        /// </summary>
        /// <param name="services"></param>
        private void ConfigureMassTransit(IServiceCollection services)
        {
            var rabbitMqSettings = Configuration.GetSection(nameof(RabbitMqSettings)).Get<RabbitMqSettings>();

            services.AddMassTransit(c =>
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

            services.AddSingleton<IHostedService, BusService>();
        }

        /// <summary>
        /// Done
        /// </summary>
        /// <param name="ConfigureAppServices"></param>
        private void ConfigureAppServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddScoped<IClientInfoGrabber, ClientInfoGrabber>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors(Constants.DefaultCorsPolicy);
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseOpenApi();
            app.UseSwaggerUi3();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
