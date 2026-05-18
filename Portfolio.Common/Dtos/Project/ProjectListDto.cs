using Portfolio.Common.Interface;

namespace Portfolio.Common.Dtos;

public class ProjectListDto : IListDto
{
    public int Id { get; set; }
    public string IconClass { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? GitHubUrl { get; set; }
    public string? WebSiteUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }
}
