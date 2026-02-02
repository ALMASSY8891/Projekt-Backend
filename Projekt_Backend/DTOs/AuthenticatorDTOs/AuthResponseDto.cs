namespace Projekt_Backend.Dtos.AuthenticatorDTOs
{
    
    public sealed class AuthResponseDto
    {
        public int ClientId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
