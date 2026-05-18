using Portfolio.Common.Interface;

namespace Portfolio.Common.Dtos;

public class ProjectSkillUpdateDto : IUpdateDto
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int SkillId { get; set; }
    public bool IsActive { get; set; }
    public DateTime UpdatedTime { get; set; } = DateTime.UtcNow;
}
