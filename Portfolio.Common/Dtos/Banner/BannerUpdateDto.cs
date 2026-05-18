using Portfolio.Common.Interface;

namespace Portfolio.Common.Dtos;

public class BannerUpdateDto : IUpdateDto
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime UpdatedTime { get; set; } = DateTime.UtcNow;
}
