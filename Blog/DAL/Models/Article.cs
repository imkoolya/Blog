namespace Blog.DAL.Models
{
    public class Article
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public int UserId { get; set; }
        public required User User { get; set; }
        public required ICollection<Tag> Tags { get; set; }
    }
}
