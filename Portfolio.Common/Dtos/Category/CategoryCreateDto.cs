using Portfolio.Common.Interface;

namespace Portfolio.Common.Dtos;

public class CategoryCreateDto : ICreateDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}
