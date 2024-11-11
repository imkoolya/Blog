namespace Blog.DAL.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required ICollection<Article> Articles { get; set; }
    }
}
