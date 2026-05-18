using Portfolio.Common.Interface;

namespace Portfolio.Common.Dtos;

public class ExperienceTypeListDto : IListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }
}
