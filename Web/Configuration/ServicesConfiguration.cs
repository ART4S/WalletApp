using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Web.Common;
using Web.Data;
using Web.Helpers;
using Web.HostedServices;
using Web.Services.Implementations;
using Web.Services.Interfaces;
using Web.Settings;

namespace Web.Configuration
{
    static class ServicesConfiguration
    {
        public static void RegisterInMemoryDataAccess(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("walletapp_db"));
            services.AddScoped<IDbContext, AppDbContext>();
        }

        public static void RegisterAppServices(this IServiceCollection services)
        {
            services.AddScoped<IWalletService, WalletService>();

            services.AddScoped<ICurrencyRatesProvider, CurrencyRatesProvider>();
        }

        public static void RegisterMapperProfiles(this IServiceCollection services)
        {
            services.AddAutoMapper(config => config.AddMaps(typeof(Startup)));
        }

        public static void RegisterHostedServices(this IServiceCollection services)
        {
            services.AddHostedService<UpdateCurrencyRatesHostedService>();
        }

        public static void RegisterCommonServices(this IServiceCollection services)
        {
            services.AddSingleton<IDateTime, SystemDateTime>();
        }

        public static void RegisterSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<UpdateCurrencyRatesSettings>(
                configuration.GetSection("UpdateCurrencyRatesSettings"));
        }
    }
}
