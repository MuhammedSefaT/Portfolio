using Portfolio.Common.Interface;

namespace Portfolio.Common.Dtos;

public class SocialMediaUpdateDto : IUpdateDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string IconClass { get; set; } = null!;
    public string ContactUrl { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime UpdatedTime { get; set; } = DateTime.UtcNow;
}
