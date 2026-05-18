namespace Portfolio.Entity;

public class ExperienceType : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public List<Experience> Experiences { get; set; } = [];
}
