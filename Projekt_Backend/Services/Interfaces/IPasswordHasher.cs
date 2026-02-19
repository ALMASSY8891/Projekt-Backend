namespace Projekt_Backend.Services.Interfaces
{
    public interface IPasswordHasher
    {
        
        int Iterations { get; }
        (string HashBase64, string SaltBase64) HashPassword(string password);
        bool VerifyPassword(string password, string storedHashBase64, string storedSaltBase64, int iterations);
    }
}
