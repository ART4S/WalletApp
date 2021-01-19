using System.Linq;
using FluentValidation;
using Web.Models.Wallets;

namespace Web.Validators.Wallets
{ 
    public class WalletTransferDtoValidator : AbstractValidator<WalletTransferDto>
    {
        public WalletTransferDtoValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(x => 0)
                .WithMessage("Should be greather than 0");

            RuleFor(x => x.CurrencyFrom).Must(BeInCurrencyCodes)
                .WithMessage("Invalid code");

            RuleFor(x => x.CurrencyTo).Must(BeInCurrencyCodes)
                .WithMessage("Invalid code");

            RuleFor(x => x).Must(CurrencyCodesAreDifferent)
                .WithMessage("Currencies must be different");
        }

        private static bool BeInCurrencyCodes(string currencyCode)
        {
            return AppConstants.CurrencyCodes.Contains(currencyCode);
        }

        private static bool CurrencyCodesAreDifferent(WalletTransferDto dto)
        {
            return dto.CurrencyFrom != dto.CurrencyTo;
        }
    }
}