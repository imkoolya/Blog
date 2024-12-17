using System.ComponentModel.DataAnnotations;

namespace Blog.Data.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Это поле обязательно.")]
        [StringLength(30, ErrorMessage = "Имя должно быть не менее 3 и не более 30 символов.", MinimumLength = 3)]
        [Display(Name = "Имя")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Это поле обязательно.")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Это поле обязательно.")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Это поле обязательно.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        [Display(Name = "Подтвердить пароль")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
