using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiscussionForum.Models
{
    public class Post
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(4000)]
        public string Content { get; set; }
        
        [Required]
        public int ThreadId { get; set; }

        [ForeignKey("ThreadId")]
        public ForumThread? Thread { get; set; }
        
        [Required]
        public string AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public ApplicationUser? Author { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? UpdatedAt { get; set; }
    }
    public class PostCreateViewModel
    {
        [Required]
        [StringLength(4000)]
        public string Content { get; set; }

        [Required]
        public int ThreadId { get; set; }
    }

}