using DiscussionForum.Data;
using DiscussionForum.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscussionForum.Controllers;

public class CategoryController : Controller
{
    private readonly ApplicationDbContext _context;
    private const int PageSize = 10;

    public CategoryController(ApplicationDbContext context)
    {
        _context = context;
    }

    private async Task<Category?> GetCategoryById(int? id)
    {
        if (id == null)
            return null;

        return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories.ToListAsync();
        return View(categories);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int? id)
    {
        var category = await GetCategoryById(id);
        if (category == null) return NotFound();
        return View(category);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Threads(int? id, int pageNumber = 1)
    {
        var category = await GetCategoryById(id);
        if (category == null) return NotFound();

        var threadsQuery = _context.ForumThreads
            .Include(t => t.Author)
            .Where(t => t.CategoryId == category.Id)
            .OrderByDescending(t => t.CreatedAt);

        var totalThreads = await threadsQuery.CountAsync();
        var totalPages = (int)Math.Ceiling(totalThreads / (double)PageSize);
        pageNumber = Math.Clamp(pageNumber, 1, Math.Max(totalPages, 1));

        var threads = await threadsQuery
            .Skip((pageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        ViewData["CategoryName"] = category.Name;
        ViewData["CategoryId"] = category.Id;
        ViewData["CurrentPage"] = pageNumber;
        ViewData["TotalPages"] = totalPages;
        ViewData["Threads"] = threads;
        ViewData["HasThreads"] = totalThreads > 0;

        return View(category);
    }

    [Authorize(Policy = "RequireAdminRole")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Create(Category category)
    {
        if (ModelState.IsValid)
        {
            _context.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }

    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Edit(int? id)
    {
        var category = await GetCategoryById(id);
        if (category == null) return NotFound();
        return View(category);
    }

    [HttpPost]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Edit(int id, Category category)
    {
        if (id != category.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id)) return NotFound();
                else throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }
    
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> Delete(int? id)
    {
        var category = await GetCategoryById(id);
        if (category == null) return NotFound();
        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Policy = "RequireAdminRole")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category != null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool CategoryExists(int id)
    {
        return _context.Categories.Any(e => e.Id == id);
    }
}
