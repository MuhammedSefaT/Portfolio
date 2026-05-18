using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Entity;

namespace Portfolio.DataAccess.Configurations;

public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).IsRequired().HasMaxLength(250);
        builder.Property(x => x.Issuer).IsRequired().HasMaxLength(250);
        builder.Property(x => x.VerificationUrl).HasMaxLength(500);
        builder.Property(x => x.ImageUrl).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000);
    }
}
