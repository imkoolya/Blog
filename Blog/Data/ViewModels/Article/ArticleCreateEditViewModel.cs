using Blog.Data.ViewModels.Tag;
namespace Blog.Data.ViewModels.Article
{
    public class ArticleCreateEditViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Summary { get; set; } = null!;     
        public string Content { get; set; } = null!;

        public List<int> SelectedTags { get; set; } = new List<int>();
        public List<TagViewModel> AllTags { get; set; } = new List<TagViewModel>();
    }
}
