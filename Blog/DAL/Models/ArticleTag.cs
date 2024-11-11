namespace Blog.DAL.Models
{
    public class ArticleTag
    {
        public int ArticleId { get; set; }
        public required Article Article { get; set; }

        public int TagId { get; set; }
        public required Tag Tag { get; set; }
    }
}
