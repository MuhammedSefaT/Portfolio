namespace Portfolio.Entity;

public class Certificate : BaseEntity
{
    public string Title { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public DateTime EarnedDate { get; set; }
    public string? VerificationUrl { get; set; }
    public string ImageUrl { get; set; } = null!;
    public string? Description { get; set; }
}
