using Portfolio.Common.Interface;

namespace Portfolio.Common.Dtos;

public class ProjectSkillCreateDto : ICreateDto
{
    public int ProjectId { get; set; }
    public int SkillId { get; set; }
    public bool IsActive { get; set; }
}
