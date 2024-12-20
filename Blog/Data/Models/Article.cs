namespace Blog.Data.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Summary { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string AuthorId { get; set; } = null!;
        public User Author { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<Tag> Tags { get; set; } = new List<Tag>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }

}
