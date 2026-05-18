using Portfolio.Common.Interface;

namespace Portfolio.Common.Dtos;

public class ExperienceCreateDto : ICreateDto
{
    public int ExperienceId { get; set; }
    public string Name { get; set; } = null!;
    public string BusinessName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsContinuing { get; set; }
    public bool IsActive { get; set; }
}
