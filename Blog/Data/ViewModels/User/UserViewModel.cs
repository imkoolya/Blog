using System.ComponentModel.DataAnnotations;

namespace Blog.Data.ViewModels.User
{
    public class UserViewModel
    {
        public string UserId { get; set; }

        [Required(ErrorMessage = "Это поле обязательно.")]
        [StringLength(30, ErrorMessage = "Имя должно быть не менее 3 и не более 30 символов.", MinimumLength = 3)]
        [Display(Name = "Имя")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Это поле обязательно.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;
        public IList<string> AllRoles { get; set; } = new List<string>();
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
