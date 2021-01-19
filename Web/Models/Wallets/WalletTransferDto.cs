namespace Web.Models.Wallets
{
    public class WalletTransferDto
    {
        public decimal Amount { get; set; }
        public string CurrencyFrom { get; set; }
        public string CurrencyTo { get; set; }
    }
}
