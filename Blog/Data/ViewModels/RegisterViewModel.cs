using System.ComponentModel.DataAnnotations;

namespace Blog.Data.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Это поле обязательно.")]
        [StringLength(30, ErrorMessage = "Имя должно быть не менее 3 и не более 30 символов.", MinimumLength = 3)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Это поле обязательно.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Это поле обязательно.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Это поле обязательно.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }
    }
}
