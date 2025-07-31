using DiscussionForum.Data;
using DiscussionForum.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DiscussionForum.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create(int threadId)
        {
            return View(new PostCreateViewModel { ThreadId = threadId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(PostCreateViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                var post = new Post
                {
                    Content = vm.Content,
                    ThreadId = vm.ThreadId,
                    AuthorId = user?.Id,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Thread", new { id = vm.ThreadId });
            }
            return View(vm);
        }
        
        [HttpGet]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue) return NotFound();

            var post = await _context.Posts
                .Include(p => p.Thread)
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (post == null) return NotFound();

            return View(post);
        }

        [HttpPost]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();

            int threadId = post.ThreadId;
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Thread", new { id = threadId });
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue) return NotFound();

            var post = await _context.Posts.Include(p => p.Author).FirstOrDefaultAsync(p => p.Id == id);
            if (post == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var isModerator = User.IsInRole("Moderator");

            if (post.AuthorId != user?.Id && !isModerator)
                return Forbid();

            var vm = new PostCreateViewModel
            {
                Content = post.Content,
                ThreadId = post.ThreadId
            };

            ViewBag.PostId = post.Id;
            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(int id, PostCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.PostId = id;
                return View(vm);
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var isModerator = User.IsInRole("Moderator");

            if (post.AuthorId != user?.Id && !isModerator)
                return Forbid();

            post.Content = vm.Content;
            post.UpdatedAt = DateTime.UtcNow;
            _context.Update(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Thread", new { id = post.ThreadId });
        }

    }
}
