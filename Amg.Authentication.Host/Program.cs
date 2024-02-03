using Amg.Authentication.Application.Contract;
using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.Application.Services;
using Amg.Authentication.Configuration;
using Amg.Authentication.Host.ProgramSetup;
using Amg.Authentication.Host.SeedWorks;
using Amg.Authentication.Infrastructure.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System;
using System.Net;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        IConfiguration configuration = builder.Configuration;

        ProgramHelper.SetupAppSettingsConfig(builder);
        ProgramHelper.ConfigureControllers(builder);
        ProgramHelper.ConfigureDatabases(builder);
        ProgramHelper.ConfigureAuthentication(builder);
        ProgramHelper.ConfigureMassTransit(builder);

        var urlSetting = ReadHostSettings(builder.Configuration).HostAddress;

        string hostname = Dns.GetHostName();

        builder.WebHost.UseUrls(urlSetting).UseKestrel();

        builder.Services.AddMemoryCache();
        builder.Services.AddScoped<IClientInfoGrabber, ClientInfoGrabber>();

        builder.Services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "Authentication API", Version = "v1" });
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });

            //var filePath = Path.Combine(AppContext.BaseDirectory, "MyApi.xml");
            //option.IncludeXmlComments(filePath);

        });

        builder.Services.AddHttpClient(Constants.NotificationHttpClient, (provider, options) =>
        {
            var settings = provider.GetService<IOptions<HostSettings>>();
            options.BaseAddress = new Uri(settings.Value.ShopAddress);
        });

        Bootstrapper.Start(builder.Services, builder.Configuration);

        try
        {
            var app = builder.Build();

            app.Seed();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();


                app.UseSwagger(c =>
                {
                    c.RouteTemplate = "authentication/swagger/{documentname}/swagger.json";
                });
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/authentication/swagger/v1/swagger.json", "My Cool API V1");
                    c.RoutePrefix = "authentication/swagger";
                });
            }

            app.UseRouting();
            app.UseCors(Constants.DefaultCorsPolicy);
            if (app.Environment.IsProduction())
            {
                app.UseHttpsRedirection();
            }
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Run();
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Failed to initialize HostBuilder {exception.Message}");
        }

        static HostSettings ReadHostSettings(IConfiguration configuration)
        {
            return configuration.GetSection(nameof(HostSettings)).Get<HostSettings>();
        }
    }
}