namespace Blog.Data.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<Article> Articles { get; set; } = new List<Article>();
    }

}
