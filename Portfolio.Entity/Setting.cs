namespace Portfolio.Entity;

public class Setting : BaseEntity
{
    public string Title { get; set; } = null!;
    public string Key { get; set; } = null!;
    public string? Value { get; set; }
    public string InputType { get; set; } = null!;
    public string? Options { get; set; }
    public string? Description { get; set; }
}
