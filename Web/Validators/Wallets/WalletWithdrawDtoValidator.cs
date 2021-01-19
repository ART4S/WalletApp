using System.Linq;
using FluentValidation;
using Web.Models.Wallets;

namespace Web.Validators.Wallets
{
    public class WalletWithdrawDtoValidator : AbstractValidator<WalletWithdrawDto>
    {
        public WalletWithdrawDtoValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(x => 0)
                .WithMessage("Should be greather than 0");

            RuleFor(x => x.Currency).Must(BeInCurrencyCodes)
                .WithMessage("Invalid code");
        }

        private static bool BeInCurrencyCodes(string currencyCode)
        {
            return AppConstants.CurrencyCodes.Contains(currencyCode);
        }
    }
}
