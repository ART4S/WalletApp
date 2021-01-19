using System;

namespace Web.Data.Entities
{
    class Wallet
    {
        public decimal Total { get; set; }
        public string CurrencyCode { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
