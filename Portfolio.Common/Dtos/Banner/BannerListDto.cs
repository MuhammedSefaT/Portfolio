using Portfolio.Common.Interface;

namespace Portfolio.Common.Dtos;

public class BannerListDto : IListDto
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }
}
