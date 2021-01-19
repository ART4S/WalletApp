using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCrontab;
using Web.Common;
using Web.Helpers;
using Web.Settings;

namespace Web.HostedServices
{
    class UpdateCurrencyRatesHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IDateTime _dateTime;
        private readonly IServiceScopeFactory _scopeFactory;

        public UpdateCurrencyRatesHostedService(
            ILogger<UpdateCurrencyRatesHostedService> logger,
            IDateTime dateTime,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _dateTime = dateTime;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            DateTime nextRunTime = default;

            while (!stoppingToken.IsCancellationRequested)
            {
                if (nextRunTime < _dateTime.Now)
                {
                    using IServiceScope scope = _scopeFactory.CreateScope();

                    var settings = scope.ServiceProvider
                        .GetRequiredService<IOptionsSnapshot<UpdateCurrencyRatesSettings>>().Value;

                    nextRunTime = GetNextRunTime(settings.IntervalCron);

                    var currencyUpdater = ActivatorUtilities
                        .GetServiceOrCreateInstance<CurrencyRatesProvider>(scope.ServiceProvider);

                    await currencyUpdater.RefreshRatesAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }

        private DateTime GetNextRunTime(string cronExpression)
        {
            CrontabSchedule schedule = null;

            try
            {
                schedule = CrontabSchedule.Parse(cronExpression);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Wrong cron format: {0}", cronExpression);
            }

            schedule ??= CrontabSchedule.Parse("0 * * * *"); // hourly

            return schedule.GetNextOccurrence(_dateTime.Now);
        }
    }
}