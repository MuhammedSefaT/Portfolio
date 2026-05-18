namespace Portfolio.Entity;

public class Experience : BaseEntity
{
    public int ExperienceId { get; set; }
    public string Name { get; set; } = null!;
    public string BusinessName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsContinuing { get; set; }

    public ExperienceType ExperienceType { get; set; } = null!;
}
