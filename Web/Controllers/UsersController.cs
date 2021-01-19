using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Models.Wallets;
using Web.Services.Interfaces;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public UsersController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet("{userId:guid}/wallet")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(IList<WalletInfoDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWallets(Guid userId)
        {
            return Ok(await _walletService.GetAll(userId));
        }

        [HttpPut("{userId:guid}/wallet/deposit")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Deposit(
            Guid userId, 
            [FromBody][Required] WalletDepositDto wallet)
        {
            await _walletService.Deposit(userId, wallet);

            return NoContent();
        }

        [HttpPut("{userId:guid}/wallet/withdraw")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Withdraw(
            Guid userId,
            [FromBody][Required] WalletWithdrawDto wallet)
        {
            await _walletService.Withdraw(userId, wallet);

            return NoContent();
        }

        [HttpPut("{userId:guid}/wallet/transfer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Transfer(
            Guid userId,
            [FromBody][Required] WalletTransferDto wallet)
        {
            await _walletService.Transfer(userId, wallet);

            return NoContent();
        }
    }
}