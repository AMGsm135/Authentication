using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.Application.Services;
using Amg.Authentication.Application.Services.CashServices;
using Amg.Authentication.Application.Services.MessageSenders;
using Amg.Authentication.CommandHandler.Modules.Accounting;
using Amg.Authentication.Infrastructure.Base;
using Amg.Authentication.Infrastructure.CrossCutting;
using Amg.Authentication.Persistence.Contexts;
using Amg.Authentication.Persistence.Repositories;
using Amg.Authentication.QueryService.Modules.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Amg.Authentication.Configuration
{
    public static class Bootstrapper
    {
        public static void Start(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSpecialTypes();

            services.SetupRepositories();
            services.SetupApplicationServices();
            services.SetupCommandServices();
            services.SetupQueryServices();
        }

        private static void AddSpecialTypes(this IServiceCollection services)
        {
            services.AddScoped(typeof(LazyDependency<>));
        }

        private static void SetupRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.Scan(s => s.FromAssemblyOf<PermissionRepository>()
                .AddClasses(c => c
                    .AssignableTo<IRepository>())
                .AsMatchingInterface()
                .WithScopedLifetime());
            
        }

        private static void SetupApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IEmailSender, ConsoleEmailSender>();
            services.AddScoped<ISmsSender, NotificationServiceSmsSender>();
            services.AddScoped<ICacheService, CacheService>();
                //.Decorate<ISmsSender, SmsSenderThrottleLimitingDecorator>();

            services.AddSingleton<RedisClient>();

            services.Scan(s => s.FromAssemblyOf<SignInService>()
                .AddClasses(c => c
                    .AssignableTo<IApplicationService>())
                .AsMatchingInterface()
                .WithScopedLifetime());
        }

        private static void SetupCommandServices(this IServiceCollection services)
        {
            services.AddScoped<ICommandBus, InMemoryCommandBus>();

            services.Scan(s => s.FromAssemblyOf<AccountsCommandHandler>()
                .AddClasses(c => c
                    .AssignableTo<ICommandHandler>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());
        }

        private static void SetupQueryServices(this IServiceCollection services)
        {
            services.Scan(s => s.FromAssemblyOf<PermissionQueryService>()
                .AddClasses(c => c
                    .AssignableTo<IQueryService>())
                .AsMatchingInterface()
                .WithScopedLifetime());
        }

    }
}
