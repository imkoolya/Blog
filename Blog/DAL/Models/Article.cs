namespace Blog.DAL.Models
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<Tag> Tags { get; set; }
    }
}
