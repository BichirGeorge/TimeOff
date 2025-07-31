using Microsoft.AspNetCore.Identity;

namespace DiscussionForum.Data;

public static class RoleSeeder
{
    public static readonly string AdministratorRole = "Administrator";
    public static readonly string ModeratorRole = "Moderator";
    public static readonly string UserRole = "User";
    
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync(AdministratorRole))
        {
            await roleManager.CreateAsync(new IdentityRole(AdministratorRole));
        }
        
        if (!await roleManager.RoleExistsAsync(ModeratorRole))
        {
            await roleManager.CreateAsync(new IdentityRole(ModeratorRole));
        }
        
        if (!await roleManager.RoleExistsAsync(UserRole))
        {
            await roleManager.CreateAsync(new IdentityRole(UserRole));
        }
    }
}
