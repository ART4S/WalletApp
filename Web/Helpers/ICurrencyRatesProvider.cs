using System.Collections.Generic;

namespace Web.Helpers
{
    interface ICurrencyRatesProvider
    {
        IDictionary<string, decimal> GetRates();
    }
}