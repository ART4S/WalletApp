using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Web.Data.Entities.Configuration
{
    class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.HasKey(x => new {x.CurrencyCode, x.UserId});

            builder.HasOne(x => x.User)
                .WithMany()
                .IsRequired();
        }
    }
}
