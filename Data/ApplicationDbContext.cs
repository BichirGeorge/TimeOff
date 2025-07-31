using DiscussionForum.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DiscussionForum.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<ForumThread> ForumThreads { get; set; } = null!;
        public DbSet<Post> Posts { get; set; } = null!;
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.Entity<ForumThread>()
                .HasOne(t => t.Category)
                .WithMany()
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.Entity<ForumThread>()
                .HasOne(t => t.Author)
                .WithMany()
                .HasForeignKey(t => t.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
                
            builder.Entity<Post>()
                .HasOne(p => p.Thread)
                .WithMany(t => t.Posts)
                .HasForeignKey(p => p.ThreadId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.Entity<Post>()
                .HasOne(p => p.Author)
                .WithMany()
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
                
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "General Discussion", Description = "General topics and discussions" },
                new Category { Id = 2, Name = "News & Announcements", Description = "Latest news and site announcements" },
                new Category { Id = 3, Name = "Technology", Description = "Discussions about technology and gadgets" },
                new Category { Id = 4, Name = "Sports", Description = "Sports related discussions" },
                new Category { Id = 5, Name = "Entertainment", Description = "Movies, music, TV shows and more" }
            );
        }
    }
}
