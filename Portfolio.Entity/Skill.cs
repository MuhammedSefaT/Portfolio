namespace Portfolio.Entity;

public class Skill : BaseEntity
{
    public string Name { get; set; } = null!;

    public List<ProjectSkill> Skills { get; set; } = [];
}
