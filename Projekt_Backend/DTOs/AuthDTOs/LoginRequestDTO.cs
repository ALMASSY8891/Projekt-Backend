using System.ComponentModel.DataAnnotations;

namespace Projekt_Backend.DTOs.AuthDTOs
{
    public sealed class LoginRequestDto
    {
        // email normalizálás a controllerben történik
        [Required, EmailAddress, StringLength(40)]
        public string Email { get; set; } = string.Empty;
        // jelszó validáció a controllerben történik (hash+salt összehasonlítás)
        [Required, StringLength(128, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;
    }
}

