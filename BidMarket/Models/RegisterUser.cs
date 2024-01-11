using System.ComponentModel.DataAnnotations;

namespace BidMarket.Models
{
    public class RegisterUser
    {
        [Required(ErrorMessage = "Поле имя должно быть заполнено")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле пароль должно быть заполнено")]
        [MinLength(6, ErrorMessage = "Минимальная длина пароля - 6")]
        public string PasswordHash { get; set; }
        [EmailAddress (ErrorMessage = "Некорректный email")]

        [Required(ErrorMessage = "Поле email должно быть заполнено")]
        public string Email { get; set; }
        [Phone (ErrorMessage = "Некорректный номер телефона")]

        [Required(ErrorMessage = "Поле телефон должно быть заполнено")]
        public string PhoneNumber { get; set; }

    }
}
