using DiscussionForum.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscussionForum.Controllers;

[Authorize(Policy = "RequireAdminRole")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    
    public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }
    
    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        var userViewModels = new List<UserRoleViewModel>();
        
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var userViewModel = new UserRoleViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles.ToList()
            };
            
            userViewModels.Add(userViewModel);
        }
        
        return View(userViewModels);
    }
    
    [HttpGet]
    public async Task<IActionResult> EditUserRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        
        if (user == null)
        {
            return NotFound();
        }
        
        var model = new EditUserRolesViewModel
        {
            UserId = user.Id,
            UserName = user.UserName
        };
        
        foreach (var role in _roleManager.Roles)
        {
            var userRoleViewModel = new UserRoleCheckboxViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name,
                IsSelected = await _userManager.IsInRoleAsync(user, role.Name)
            };
            
            model.Roles.Add(userRoleViewModel);
        }
        
        return View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> EditUserRoles(EditUserRolesViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);
        
        if (user == null)
        {
            return NotFound();
        }
        
        var userRoles = await _userManager.GetRolesAsync(user);
        var result = await _userManager.RemoveFromRolesAsync(user, userRoles);
        
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Cannot remove user from existing roles");
            return View(model);
        }
        
        result = await _userManager.AddToRolesAsync(user, 
            model.Roles.Where(x => x.IsSelected).Select(y => y.RoleName));
        
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Cannot add user to selected roles");
            return View(model);
        }
        
        return RedirectToAction(nameof(Index));
    }
}
