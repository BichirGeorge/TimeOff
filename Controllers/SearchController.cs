using DiscussionForum.Data;
using DiscussionForum.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscussionForum.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;
        public SearchController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> Index(string query)
        {
            ViewData["SearchQuery"] = query;

            if (string.IsNullOrWhiteSpace(query))
                return View(new SearchResultsViewModel { Query = query });

            var threads = await _context.ForumThreads
                .Where(t => t.Title.Contains(query) || t.Description.Contains(query))
                .Include(t => t.Category)
                .Include(t => t.Author)
                .ToListAsync();

            var posts = await _context.Posts
                .Where(p => p.Content.Contains(query))
                .Include(p => p.Thread)
                .Include(p => p.Author)
                .ToListAsync();

            var vm = new SearchResultsViewModel
            {
                Query = query,
                Threads = threads,
                Posts = posts
            };
            return View(vm);
        }
    }
}