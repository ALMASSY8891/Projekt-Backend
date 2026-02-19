using System.ComponentModel.DataAnnotations;

namespace Projekt_Backend.DTOs.AuthDTOs
{
    public sealed class RegisterRequestDto
    {
        // név validáció a controllerben történik (pl. trim, üres string ellenőrzés)
        [Required, StringLength(30, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
        // email normalizálás a controllerben történik
        [Required, EmailAddress, StringLength(40)]
        public string Email { get; set; } = string.Empty;
        // jelszó validáció a controllerben történik (hash+salt összehasonlítás)
        [Required, StringLength(128, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;
        // telefonszám validáció a controllerben történik (pl. trim, üres string ellenőrzés)
        [StringLength(40)]
        public string? Telephone { get; set; }
        // számlázási cím validáció a controllerben történik (pl. trim, üres string ellenőrzés)
        [StringLength(50)]
        public string? BillingAddress { get; set; }
    }
}

