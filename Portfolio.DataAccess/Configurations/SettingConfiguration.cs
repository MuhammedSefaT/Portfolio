using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Entity;

namespace Portfolio.DataAccess.Configurations;

public class SettingConfiguration : IEntityTypeConfiguration<Setting>
{
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).IsRequired().HasMaxLength(250);
        builder.Property(x => x.Key).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Value).HasMaxLength(1000);
        builder.Property(x => x.InputType).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Options).HasMaxLength(1000);
        builder.Property(x => x.Description).HasMaxLength(500);
    }
}
