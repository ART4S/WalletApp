using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Web.Data;
using Web.Data.Entities;
using Web.Exceptions;
using Web.Helpers;
using Web.Models.Wallets;
using Web.Services.Interfaces;

namespace Web.Services.Implementations
{
    class WalletService : IWalletService
    {
        private readonly ILogger _logger;
        private readonly IDbContext _db;
        private readonly IMapper _mapper;
        private readonly ICurrencyRatesProvider _currencyRatesProvider;

        public WalletService(
            ILogger<WalletService> logger, 
            IDbContext db, 
            IMapper mapper, 
            ICurrencyRatesProvider currencyRatesProvider)
        {
            _logger = logger;
            _db = db;
            _mapper = mapper;
            _currencyRatesProvider = currencyRatesProvider;
        }

        public async Task<IList<WalletInfoDto>> GetAll(Guid userId)
        {
            List<WalletInfoDto> wallets = await _db.Wallets
                .Where(x => x.UserId == userId)
                .AsNoTracking()
                .ProjectTo<WalletInfoDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return wallets;
        }

        public async Task Deposit(Guid userId, WalletDepositDto walletDto)
        {
            await CheckUserExists(userId);

            Wallet wallet = await CreateWalletIfNotExist(userId, walletDto.Currency);

            if (!TryAddAmount(wallet.Total, walletDto.Amount, out decimal newTotal, out string error))
            {
                throw new InvalidRequestException(error);
            }

            wallet.Total = newTotal;

            await _db.SaveChangesAsync();
        }

        public async Task Withdraw(Guid userId, WalletWithdrawDto walletDto)
        {
            await CheckUserExists(userId);

            Wallet wallet = await CreateWalletIfNotExist(userId, walletDto.Currency);

            if (!TrySubstractAmount(wallet.Total, walletDto.Amount, out decimal newTotal, out string error))
            {
                throw new InvalidRequestException(error);
            }

            wallet.Total = newTotal;

            await _db.SaveChangesAsync();
        }

        public async Task Transfer(Guid userId, WalletTransferDto walletDto)
        {
            await CheckUserExists(userId);

            Wallet walletFrom = await CreateWalletIfNotExist(userId, walletDto.CurrencyFrom);

            if (!TrySubstractAmount(walletFrom.Total, walletDto.Amount, out decimal newTotalFrom , out string error))
            {
                throw new InvalidRequestException(error);
            }

            walletFrom.Total = newTotalFrom;

            var converter = new CurrencyConverter(_logger, _currencyRatesProvider);

            if (!converter.TryConvert(
                walletDto.CurrencyFrom,
                walletDto.CurrencyTo,
                walletDto.Amount,
                out decimal amountTo,
                out error))
            {
                throw new InvalidRequestException(error);
            }

            Wallet walletTo = await CreateWalletIfNotExist(userId, walletDto.CurrencyTo);

            if (!TryAddAmount(walletTo.Total, amountTo, out decimal newTotalTo, out error))
            {
                throw new InvalidRequestException(error);
            }

            walletTo.Total = newTotalTo;

            await _db.SaveChangesAsync();
        }

        private async Task CheckUserExists(Guid userId)
        {
            if (!await _db.Users.AnyAsync(x => x.Id == userId))
            {
                throw new NotFoundException("User", userId);
            }
        }

        private async Task<Wallet> CreateWalletIfNotExist(Guid userId, string currencyCode)
        {
            currencyCode = currencyCode.ToLower();

            Wallet wallet = await _db.Wallets
                .SingleOrDefaultAsync(x => x.UserId == userId && x.CurrencyCode == currencyCode);

            if (wallet is null)
            {
                wallet = new Wallet
                {
                    UserId = userId,
                    CurrencyCode = currencyCode
                };

                await _db.Wallets.AddAsync(wallet);
            }

            return wallet;
        }

        private static bool TryAddAmount(
            decimal total, 
            decimal amount, 
            out decimal result, 
            out string error)
        {
            result = default;
            error = default;

            if (decimal.MaxValue - total < amount)
            {
                error = "Operation cannot be performed: amount is too large";
                return false;
            }

            result = total + amount;
            return true;
        }

        private static bool TrySubstractAmount(
            decimal total, 
            decimal amount, 
            out decimal result, 
            out string error)
        {
            result = default;
            error = default;

            if (amount > total)
            {
                error = "Operation cannot be performed: amount is too large";
                return false;
            }

            result = total - amount;
            return true;
        }
    }
}
