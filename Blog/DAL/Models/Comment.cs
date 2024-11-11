namespace Blog.DAL.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public required string Content { get; set; }
        public int ArticleId { get; set; }
        public required Article Article { get; set; }
        public int UserId { get; set; }
        public required User User { get; set; }
    }
}
