using System.ComponentModel.DataAnnotations;

namespace Projekt_Backend.DTOs.AuthDTOs
{
    public sealed class RegisterRequestDto
    {
        [Required(ErrorMessage = "A név megadása kötelező.")]
        [StringLength(30, MinimumLength = 2,
            ErrorMessage = "A név hossza 2 és 30 karakter között kell legyen.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Az email megadása kötelező.")]
        [EmailAddress(ErrorMessage = "Az email formátuma nem megfelelő.")]
        [StringLength(40, ErrorMessage = "Az email legfeljebb 40 karakter lehet.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A jelszó megadása kötelező.")]
        [StringLength(128, MinimumLength = 8,
            ErrorMessage = "A jelszónak 8 és 128 karakter között kell lennie.")]
        public string Password { get; set; } = string.Empty;

        [StringLength(40, ErrorMessage = "A telefonszám legfeljebb 40 karakter lehet.")]
        public string? Telephone { get; set; }

        [StringLength(50, ErrorMessage = "A számlázási cím legfeljebb 50 karakter lehet.")]
        public string? BillingAddress { get; set; }
    }
}
