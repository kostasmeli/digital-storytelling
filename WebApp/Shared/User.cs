using System.ComponentModel.DataAnnotations;


namespace BlazorApp.Shared
{
    public class User
    {
        [Required(ErrorMessage = "Το Όνομα είναι υποχρεωτικό")]
        public string FirstName { get; set; } = string.Empty;


        [Required(ErrorMessage = "Το Επώνυμο είναι υποχρεωτικό")]
        public string LastName { get; set; } = string.Empty;


        [Required(ErrorMessage = "Το email είναι υποχρεωτικό")]
        [EmailAddress(ErrorMessage = "Λανθασμένη μορφή email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Το όνομα χρήστη είναι υποχρεωτικό")]
        [Key]
        public string Username { get; set; }

        [Required(ErrorMessage = "Ο κωδικός είναι υποχρεωτικός")]
        [StringLength(100, ErrorMessage = "Ο κωδικός πρέπει να έχει τουλάχιστον {2} χαρακτήρες.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "User";

		public virtual ICollection<DialogueSession>? DialogueSessions { get; set; }
	}
}
