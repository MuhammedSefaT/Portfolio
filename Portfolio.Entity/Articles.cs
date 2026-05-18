namespace Portfolio.Entity;

public class Articles : BaseEntity
{
    public int CategoryId { get; set; }
    public string Title { get; set; } = null!;
    public string ShortDescription { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int ViewCount { get; set; }

    public Category Category { get; set; } = null!;
}
