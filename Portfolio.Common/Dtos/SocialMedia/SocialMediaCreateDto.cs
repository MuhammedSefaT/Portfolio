using Portfolio.Common.Interface;

namespace Portfolio.Common.Dtos;

public class SocialMediaCreateDto : ICreateDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string IconClass { get; set; } = null!;
    public string ContactUrl { get; set; } = null!;
    public bool IsActive { get; set; }
}
