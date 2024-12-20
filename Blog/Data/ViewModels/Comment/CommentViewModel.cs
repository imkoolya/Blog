namespace Blog.Data.ViewModels.Comment
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public string Author { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
