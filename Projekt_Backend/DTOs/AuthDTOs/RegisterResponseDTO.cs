namespace Projekt_Backend.DTOs.AuthDTOs
{
    public sealed class RegisterResponseDto
    {
        public int ClientId { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
