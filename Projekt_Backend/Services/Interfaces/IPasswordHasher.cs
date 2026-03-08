namespace Projekt_Backend.Services.Interfaces
{
    // Ez az interface a jelszó hash-eléséért és ellenőrzéséért felelős szolgáltatást definiálja. A jelszó hash-elése során egy véletlenszerű sót (salt) generálunk, majd a jelszót és a sót egy iterációs számmal együtt használjuk a hash-eléshez. Az ellenőrzés során a megadott jelszót ugyanazzal a sóval és iterációs számmal hash-eljük, majd összehasonlítjuk a tárolt hash értékével.
    public interface IPasswordHasher
    {
        
        int Iterations { get; }
        (string HashBase64, string SaltBase64) HashPassword(string password);
        bool VerifyPassword(string password, string storedHashBase64, string storedSaltBase64, int iterations);
    }
}
