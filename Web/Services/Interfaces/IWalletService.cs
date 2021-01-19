using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Models.Wallets;

namespace Web.Services.Interfaces
{
    public interface IWalletService
    {
        Task<IList<WalletInfoDto>> GetAll(Guid userId);
        Task Deposit(Guid userId, WalletDepositDto wallet);
        Task Withdraw(Guid userId, WalletWithdrawDto wallet);
        Task Transfer(Guid userId, WalletTransferDto wallet);
    }
}
