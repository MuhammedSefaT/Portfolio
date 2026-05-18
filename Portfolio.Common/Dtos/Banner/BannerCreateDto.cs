using Portfolio.Common.Interface;

namespace Portfolio.Common.Dtos;

public class BannerCreateDto : ICreateDto
{
    public string ImageUrl { get; set; } = null!;
    public bool IsActive { get; set; }
}
