using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Entity;

namespace Portfolio.DataAccess.Configurations;

public class SocialMediaConfiguration : IEntityTypeConfiguration<SocialMedia>
{
    public void Configure(EntityTypeBuilder<SocialMedia> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).IsRequired().HasMaxLength(150);
        builder.Property(x => x.IconClass).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ContactUrl).IsRequired().HasMaxLength(500);
        builder.Property(x => x.Description).HasMaxLength(500);
    }
}
