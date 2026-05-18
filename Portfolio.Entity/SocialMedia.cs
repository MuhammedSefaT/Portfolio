namespace Portfolio.Entity;

public class SocialMedia : BaseEntity
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string IconClass { get; set; } = null!;
    public string ContactUrl { get; set; } = null!;
}
