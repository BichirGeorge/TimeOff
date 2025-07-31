using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscussionForum.Controllers;

[Authorize(Policy = "RequireModeratorRole")]
public class ModeratorController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult ManageContent()
    {
        return View();
    }
}
