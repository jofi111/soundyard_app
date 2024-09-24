using System.ComponentModel.DataAnnotations;

namespace club.soundyard.web.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Heslo")]
        public string Password { get; set; }

        [Display(Name = "Zapamatovat si mě?")]
        public bool RememberMe { get; set; }

        // Odkaz na registrační stránku
        public string RegisterUrl { get; set; } = "/Account/Register";
    }
}
