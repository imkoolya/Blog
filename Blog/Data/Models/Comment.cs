namespace Blog.Data.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public Article Article { get; set; } = null!;
        public User User { get; set; } = null!;

    }
}
