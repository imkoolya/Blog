using System.ComponentModel.DataAnnotations;

namespace Blog.Data.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Это поле обязательно.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Это поле обязательно.")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = null!;

        [Display(Name = "Запомнить меня?")]
        public bool RememberMe { get; set; }
    }
}
