using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Web.Settings;

namespace Web.Helpers
{
    class CurrencyRatesProvider : ICurrencyRatesProvider
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UpdateCurrencyRatesSettings _settings;

        private static readonly ConcurrentDictionary<string, decimal> Rates
            = new ConcurrentDictionary<string, decimal> {["eur"] = 1};

        public CurrencyRatesProvider(
            ILogger<CurrencyRatesProvider> logger,
            IOptionsSnapshot<UpdateCurrencyRatesSettings> settings,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _settings = settings.Value;
            _httpClientFactory = httpClientFactory;
        }

        public IDictionary<string, decimal> GetRates()
        {
            return Rates;
        }

        public async Task RefreshRatesAsync()
        {
            string uri = _settings.Uri;

            HttpClient client = _httpClientFactory.CreateClient();

            int retryCount = _settings.RetryCount;

            HttpResponseMessage response = null;

            while (retryCount > 0)
            {
                try
                {
                    response = await client.GetAsync(new Uri(uri));

                    response.EnsureSuccessStatusCode();

                    break;
                }
                catch (TaskCanceledException ex)
                {
                    retryCount--;

                    if (retryCount == 0)
                    {
                        _logger.LogCritical(ex, "Error while updating currency rates: timeout");

                        return;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "Error while updating currency rates");

                    return;
                }
            }

            string xml = await response!.Content.ReadAsStringAsync();

            LoadRatesFromXml(xml);
        }

        private void LoadRatesFromXml(string xml)
        {
            try
            {
                var doc = new XmlDocument();

                doc.LoadXml(xml);

                var currencyNodes = doc.SelectNodes("//*[@currency and @rate]")!.OfType<XmlNode>();

                foreach (XmlNode node in currencyNodes)
                {
                    string currency = node.Attributes!["currency"]!.Value.ToLower();

                    decimal rate = decimal.Parse(
                        node.Attributes!["rate"]!.Value,
                        CultureInfo.InvariantCulture);

                    Rates.AddOrUpdate(currency, rate, (_, __) => rate);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Error while parsing data from xml");
            }
        }
    }
}