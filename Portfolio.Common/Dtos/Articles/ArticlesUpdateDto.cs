using Portfolio.Common.Interface;

namespace Portfolio.Common.Dtos;

public class ArticlesUpdateDto : IUpdateDto
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Title { get; set; } = null!;
    public string ShortDescription { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int ViewCount { get; set; }
    public bool IsActive { get; set; }
    public DateTime UpdatedTime { get; set; } = DateTime.UtcNow;
}
