namespace Blog.Data.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public User User { get; set; } = null!;

        public List<Tag> Tags { get; set; } = new List<Tag> { };
        public List<Comment> Comments { get; set; } = new List<Comment> { };
    }
}
