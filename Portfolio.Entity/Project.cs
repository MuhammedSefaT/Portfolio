namespace Portfolio.Entity;

public class Project : BaseEntity
{
    public string IconClass { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? GitHubUrl { get; set; }
    public string? WebSiteUrl { get; set; }

    public List<ProjectSkill> Skills { get; set; } = [];
}
