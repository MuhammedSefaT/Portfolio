using Portfolio.Common.Interface;

namespace Portfolio.Common.Dtos;

public class CertificateListDto : IListDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public DateTime EarnedDate { get; set; }
    public string? VerificationUrl { get; set; }
    public string ImageUrl { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime UpdatedTime { get; set; }
}
