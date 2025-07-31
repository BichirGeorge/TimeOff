namespace DiscussionForum.Models;

public class UserRoleViewModel
{
    public string UserId { get; set; } = null!;
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
}

public class EditUserRolesViewModel
{
    public string UserId { get; set; } = null!;
    public string? UserName { get; set; }
    public List<UserRoleCheckboxViewModel> Roles { get; set; } = new List<UserRoleCheckboxViewModel>();
}

public class UserRoleCheckboxViewModel
{
    public string RoleId { get; set; } = null!;
    public string? RoleName { get; set; }
    public bool IsSelected { get; set; }
}
