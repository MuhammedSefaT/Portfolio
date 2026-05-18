using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Entity;

namespace Portfolio.DataAccess.Configurations;

public class ExperienceTypeConfiguration : IEntityTypeConfiguration<ExperienceType>
{
    public void Configure(EntityTypeBuilder<ExperienceType> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(150);
        builder.Property(x => x.Description).HasMaxLength(500);
    }
}
