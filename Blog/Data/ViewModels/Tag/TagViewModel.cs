using System.ComponentModel.DataAnnotations;

namespace Blog.Data.ViewModels.Tag
{
    public class TagViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Это поле обязательно.")]
        [StringLength(30, ErrorMessage = "Имя должно быть не менее 1 и не более 30 символов.", MinimumLength = 1)]
        [Display(Name = "Название")]
        public string Name { get; set; } = null!;
        public int ArticleCount { get; set; }
    }
}
