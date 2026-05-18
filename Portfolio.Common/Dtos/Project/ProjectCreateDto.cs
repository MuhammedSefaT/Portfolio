using Portfolio.Common.Interface;

namespace Portfolio.Common.Dtos;

public class ProjectCreateDto : ICreateDto
{
    public string IconClass { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? GitHubUrl { get; set; }
    public string? WebSiteUrl { get; set; }
    public bool IsActive { get; set; }
}
