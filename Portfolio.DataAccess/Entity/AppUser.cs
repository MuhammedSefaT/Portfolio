using Microsoft.AspNetCore.Identity;

namespace Portfolio.DataAccess.Entity;

public class AppUser : IdentityUser<int>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    public string FullName => string.Join(" ", FirstName, LastName);
}
