namespace Portfolio.Entity;

public class ProjectSkill : BaseEntity
{
    public int ProjectId { get; set; }
    public int SkillId { get; set; }

    public Project Project { get; set; } = null!;
    public Skill Skill { get; set; } = null!;
}
