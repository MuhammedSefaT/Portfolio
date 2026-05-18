using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Entity;

namespace Portfolio.DataAccess.Configurations;

public class ExperienceConfiguration : IEntityTypeConfiguration<Experience>
{
    public void Configure(EntityTypeBuilder<Experience> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
        builder.Property(x => x.BusinessName).IsRequired().HasMaxLength(250);
        builder.Property(x => x.Description).IsRequired();

        builder.HasOne(x => x.ExperienceType)
            .WithMany(x => x.Experiences)
            .HasForeignKey(x => x.ExperienceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
