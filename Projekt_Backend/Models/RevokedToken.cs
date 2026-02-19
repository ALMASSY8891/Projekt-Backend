namespace Projekt_Backend.Models;

public class RevokedToken
{
    public int RevokedTokenId { get; set; }
    public string Jti { get; set; } = null!;
    public int ClientId { get; set; }
    public DateTime RevokedAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public Client Client { get; set; } = null!;
}
