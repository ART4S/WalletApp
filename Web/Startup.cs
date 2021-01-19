using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Web.Configuration;
using Web.Filters;
using Web.Middlewares;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.RegisterInMemoryDataAccess();
            services.RegisterCommonServices();
            services.RegisterAppServices();
            services.RegisterMapperProfiles();
            services.RegisterHostedServices();
            services.RegisterSettings(Configuration);

            services.AddLogging();

            services.AddControllers(options =>
                {
                    options.Filters.Add(typeof(InvalidModelFilter));
                })
                .AddFluentValidation();

            services.AddHttpClient();

            services.AddValidatorsFromAssemblyContaining<Startup>();

            services.AddScoped<ExceptionHandlerMiddleware>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
