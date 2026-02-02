using System.ComponentModel.DataAnnotations;

namespace Projekt_Backend.Dtos.AuthAuthenticatorDTOs
{
    public sealed class RegisterRequestDto
    {
   
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Telephone { get; set; } = string.Empty;

        public string BillingAddress { get; set; } = string.Empty;
    }
}


