using System.Collections.Generic;

namespace Web.Helpers
{
    interface ICurrencyRatesProvider
    {
        IReadOnlyDictionary<string, decimal> GetRates();
    }
}