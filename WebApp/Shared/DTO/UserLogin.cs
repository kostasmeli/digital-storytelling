using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Shared.DTO
{
    public class UserLogin
    {
        [Required(ErrorMessage = "Το όνομα χρήστη ειναι υποχρεωτικό")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ο κωδικός είναι υποχρεωτικός")]
        [StringLength(100, ErrorMessage = "Ο κωδικός πρέπει να έχει τουλάχιστον {2} χαρακτήρες.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

    }
}
