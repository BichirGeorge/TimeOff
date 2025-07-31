using Microsoft.AspNetCore.Identity;

namespace DiscussionForum.Models;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime JoinDate { get; set; } = DateTime.UtcNow;
}
