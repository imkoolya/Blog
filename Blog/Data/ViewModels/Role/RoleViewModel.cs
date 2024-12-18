using System.ComponentModel.DataAnnotations;

namespace Blog.Data.ViewModels.Role
{
    public class RoleViewModel
    {
        public string Id { get; set; } = null!;

        [Required(ErrorMessage = "Это поле обязательно.")]
        [StringLength(15, ErrorMessage = "Имя должно быть не менее 2 и не более 15 символов.", MinimumLength = 2)]
        [Display(Name = "Название")]
        public string Name { get; set; } = null!;
    }

}
