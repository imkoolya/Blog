using System.ComponentModel.DataAnnotations;

namespace Blog.Data.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Это поле обязательно.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Это поле обязательно.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня?")]
        public bool RememberMe { get; set; }
    }
}
