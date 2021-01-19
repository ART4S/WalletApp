namespace Web.Settings
{
    public class UpdateCurrencyRatesSettings
    {
        public string Uri { get; set; }
        public string IntervalCron { get; set; }
        public int RetryCount { get; set; }
    }
}
