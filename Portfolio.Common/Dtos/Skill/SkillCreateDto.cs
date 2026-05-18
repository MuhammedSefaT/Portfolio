using Portfolio.Common.Interface;

namespace Portfolio.Common.Dtos;

public class SkillCreateDto : ICreateDto
{
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
}
