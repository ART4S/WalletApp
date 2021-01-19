using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Web.Data.Entities;

namespace Web.Data
{
    interface IDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Wallet> Wallets { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
