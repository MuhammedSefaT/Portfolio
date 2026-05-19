namespace Portfolio.Entity;

public class Category : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public List<Article> Articles { get; set; } = [];
}
