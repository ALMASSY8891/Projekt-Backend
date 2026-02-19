namespace Projekt_Backend.DTOs.AuthDTOs
{
    public sealed class LoginResponseDto
    {
        public int ClientId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

    }
}


