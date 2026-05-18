namespace Portfolio.Entity;

public class Education : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Institution { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsContinuing { get; set; }
}
