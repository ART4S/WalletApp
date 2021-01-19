using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Web.Data.Entities;

namespace Web.Data
{
    static class AppDbContextSeed
    {
        public static async Task SeedAsync(this AppDbContext context, ILogger logger)
        {
            if (await context.Users.AnyAsync()) return;

            logger.LogInformation("start seeding data");

            IEnumerable<object> entities = typeof(AppDbContextSeed)
                .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                .Where(x => x.Name.StartsWith("Get"))
                .SelectMany(x => x.Invoke(null, new object[] { }) as IEnumerable<object>);

            await context.AddRangeAsync(entities);

            await context.SaveChangesAsync();

            logger.LogInformation("end seeding data");
        }

        private static User[] GetUsers()
        {
            return new[] {new User {Id = new Guid(AppConstants.DefaultUserId)}};
        }

        private static Wallet[] GetWallets()
        {
            return new[]
            {
                new Wallet
                {
                    UserId = new Guid(AppConstants.DefaultUserId),
                    CurrencyCode = "usd"
                },
                new Wallet
                {
                    UserId = new Guid(AppConstants.DefaultUserId),
                    CurrencyCode = "rub"
                }
            };
        }
    }
}
