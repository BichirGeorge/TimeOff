namespace DiscussionForum.Models
{
    public class SearchResultsViewModel
    {
        public string Query { get; set; }
        public List<ForumThread> Threads { get; set; } = new();
        public List<Post> Posts { get; set; } = new();
    }
}