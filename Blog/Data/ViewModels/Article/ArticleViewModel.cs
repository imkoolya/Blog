using Blog.Data.ViewModels.Comment;
using Blog.Data.ViewModels.Tag;

namespace Blog.Data.ViewModels.Article
{
    public class ArticleViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Summary { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string Author { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<TagViewModel> Tags { get; set; } = new List<TagViewModel>();
        public List<CommentViewModel> Comments { get; set; } = new List<CommentViewModel>();
    }
}
