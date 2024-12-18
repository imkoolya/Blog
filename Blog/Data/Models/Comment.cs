namespace Blog.Data.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public string AuthorId { get; set; } = null!;
        public int ArticleId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User Author { get; set; } = null!;
        public Article Article { get; set; } = null!;
    }
}
