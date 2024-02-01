using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text;
using System.Threading.Tasks;
using Amg.Authentication.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SampleWebApp.Authentication;
using SampleWebApp.Settings;

namespace SampleWebApp
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
            services.AddControllers();
            ConfigureAppSettings(services);
            ConfigureHttpClients(services);
            ConfigureAuthentication(services);
        }

        private void ConfigureAppSettings(IServiceCollection services)
        {
            services.Configure<HostSettings>(Configuration.GetSection(nameof(HostSettings)));
            services.Configure<JwtTokenSettings>(Configuration.GetSection(nameof(JwtTokenSettings)));
        }

        private void ConfigureHttpClients(IServiceCollection services)
        {
            services.AddHttpClient(Constants.AuthenticationHttpClient, (provider, options) =>
            {
                var settings = provider.GetService<IOptions<HostSettings>>();
                options.BaseAddress = new Uri(settings.Value.AuthenticationAddress);
            });
        }


        private void ConfigureAuthentication(IServiceCollection services)
        {
            var jwtTokenSettings = Configuration.GetSection(nameof(JwtTokenSettings)).Get<JwtTokenSettings>();

            services.AddScoped<PermissionAuthorizationFilter>();

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
            });

            services.AddAuthorization();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
