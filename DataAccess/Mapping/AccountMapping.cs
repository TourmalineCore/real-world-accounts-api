using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NodaTime;

namespace DataAccess.Mapping;

internal class AccountMapping : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasIndex(user => user.Login)
            .IsUnique();
        
        builder.Property(x => x.TenantId).HasDefaultValue(1L);

        builder.HasData(
                new
                {
                    Id = MappingData.AdminAccountId,
                    Login = "admin",
                    IsBlocked = false,
                    CreatedAt = Instant.FromDateTimeUtc(MappingData.AccountsCreatedAtUtc),
                    TenantId = 1L
                },
                new
                {
                    Id = MappingData.GuestAccountId,
                    Login = "guest",
                    IsBlocked = false,
                    CreatedAt = Instant.FromDateTimeUtc(MappingData.AccountsCreatedAtUtc),
                    TenantId = 2L
                }
            );
    }
}