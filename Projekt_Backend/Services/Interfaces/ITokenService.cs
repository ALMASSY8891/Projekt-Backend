namespace Projekt_Backend.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(int clientId, string email, string name, string role);
    }
}

