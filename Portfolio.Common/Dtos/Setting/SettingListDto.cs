using Portfolio.Common.Interface;

namespace Portfolio.Common.Dtos;

public class SettingListDto : IListDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Key { get; set; } = null!;
    public string? Value { get; set; }
    public string InputType { get; set; } = null!;
    public string? Options { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }
}
