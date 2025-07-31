using DiscussionForum.Data;
using DiscussionForum.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DiscussionForum.Controllers;

public class ThreadController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ThreadController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int? id, string sortBy = "dateDesc")
    {
        if (id == null)
        {
            return NotFound();
        }

        var forumThread = await _context.ForumThreads
            .Include(t => t.Category)
            .Include(t => t.Author)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (forumThread == null)
        {
            return NotFound();
        }

        var postsQuery = _context.Posts
            .Include(p => p.Author)
            .Where(p => p.ThreadId == id);

        switch (sortBy)
        {
            case "dateAsc":
                postsQuery = postsQuery.OrderBy(p => p.CreatedAt);
                break;
            case "dateDesc":
                postsQuery = postsQuery.OrderByDescending(p => p.CreatedAt);
                break;
            case "author":
                postsQuery = postsQuery.OrderBy(p => p.Author.UserName);
                break;
            default:
                postsQuery = postsQuery.OrderByDescending(p => p.CreatedAt);
                break;
        }

        var posts = await postsQuery.ToListAsync();
        
        var currentUserId = _userManager.GetUserId(User);
        ViewData["CurrentUserId"] = currentUserId;
        
        ViewData["Thread"] = forumThread;
        ViewData["Posts"] = posts;
        ViewData["CurrentSort"] = sortBy;

        return View(forumThread);
    }

    [Authorize]
    public IActionResult Create(int? categoryId)
    {
        if (categoryId == null)
        {
            return RedirectToAction("Index", "Category");
        }

        ViewData["CategoryId"] = categoryId;
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(CreateThreadViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            var newThread = new ForumThread
            {
                Title = model.Title,
                Description = model.Description,
                CategoryId = model.CategoryId,
                AuthorId = user?.Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.Add(newThread);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = newThread.Id });
        }

        ViewData["CategoryId"] = model.CategoryId;
        return View(model);
    }

    [Authorize]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var forumThread = await _context.ForumThreads
            .Include(t => t.Category)
            .Include(t => t.Author)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (forumThread == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (forumThread.AuthorId != user?.Id && !User.IsInRole("Moderator"))
        {
            return Forbid();
        }

        return View(forumThread);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var forumThread = await _context.ForumThreads.FindAsync(id);

        if (forumThread == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (forumThread.AuthorId != user?.Id && !User.IsInRole("Moderator"))
        {
            return Forbid();
        }

        _context.ForumThreads.Remove(forumThread);
        await _context.SaveChangesAsync();

        return RedirectToAction("Threads", "Category", new { id = forumThread.CategoryId });
    }
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var thread = await _context.ForumThreads.FirstOrDefaultAsync(t => t.Id == id);
        if (thread == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);
        var isModerator = User.IsInRole("Moderator");

        if (thread.AuthorId != user?.Id && !isModerator)
            return Forbid();

        var vm = new CreateThreadViewModel
        {
            Title = thread.Title,
            Description = thread.Description,
            CategoryId = thread.CategoryId
        };

        ViewBag.ThreadId = thread.Id;
        ViewData["Categories"] = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", thread.CategoryId);

        
        return View(vm);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Edit(int id, CreateThreadViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ThreadId = id;
            return View(vm);
        }

        var thread = await _context.ForumThreads.FindAsync(id);
        if (thread == null)
            return NotFound();

        var user = await _userManager.GetUserAsync(User);
        var isModerator = User.IsInRole("Moderator");

        if (thread.AuthorId != user?.Id && !isModerator)
            return Forbid();

        thread.Title = vm.Title;
        thread.Description = vm.Description;
        thread.CategoryId = vm.CategoryId;
        thread.UpdatedAt = DateTime.UtcNow;

        _context.Update(thread);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", new { id = thread.Id });
    }

}
