using System.ComponentModel.DataAnnotations;

namespace Projekt_Backend.DTOs.AuthDTOs
{// Ez a DTO a bejelentkezési kérések adatainak reprezentációjára szolgál. A kliens által küldött email és jelszó adatait tartalmazza, amelyeket a controllerben validálunk és használunk a hitelesítéshez. Az adatok validálásához DataAnnotations attribútumokat használunk, amelyek biztosítják, hogy az email és jelszó megfelelő formátumú és hosszúságú legyen.
    public sealed class LoginRequestDto
    {
        [Required(ErrorMessage = "Az email megadása kötelező.")]
        [EmailAddress(ErrorMessage = "Az email formátuma nem megfelelő.")]
        [StringLength(40, ErrorMessage = "Az email legfeljebb 40 karakter lehet.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A jelszó megadása kötelező.")]
        [StringLength(128, ErrorMessage = "A jelszó legfeljebb 128 karakter lehet.")]
        public string Password { get; set; } = string.Empty;
    }
}

