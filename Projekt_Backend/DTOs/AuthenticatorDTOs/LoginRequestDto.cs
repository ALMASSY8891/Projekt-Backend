using System.ComponentModel.DataAnnotations;

namespace Projekt_Backend.Dtos.AuthenticatorDTOs
{
    public sealed class LoginRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
