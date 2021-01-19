using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Web.Helpers
{
    class CurrencyConverter
    {
        private readonly ILogger _logger;
        private readonly ICurrencyRatesProvider _currencyRatesProvider;

        public CurrencyConverter(
            ILogger logger, 
            ICurrencyRatesProvider currencyRatesProvider)
        {
            _logger = logger;
            _currencyRatesProvider = currencyRatesProvider;
        }

        // TODO: add appropriate description for error messages
        public bool TryConvert(
            string currencyFrom, 
            string currencyTo, 
            decimal amount, 
            out decimal result, 
            out string error)
        {
            result = default;
            error = default;

            currencyFrom = currencyFrom.ToLower();
            currencyTo = currencyTo.ToLower();

            if (currencyFrom == "eur" && currencyTo == "eur")
            {
                result = amount;
                return true;
            }

            IDictionary<string, decimal> rates = _currencyRatesProvider.GetRates();

            if (!rates.TryGetValue(currencyFrom, out var fromRate))
            {
                error = "";
                return false;
            }

            if (!rates.TryGetValue(currencyTo, out var toRate))
            {
                error = "";
                return false;
            }

            try
            {
                result = amount / fromRate * toRate;
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error while converting values");

                error = "";
                return false;
            }
        }
    }
}
