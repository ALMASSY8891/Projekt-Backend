using System.Security.Cryptography;
using Projekt_Backend.Services.Interfaces;

namespace Projekt_Backend.Services
{
    public sealed class PasswordHasher : IPasswordHasher
    {
        // Az iterációk száma egy kompromisszum a biztonság és a teljesítmény között. Minél több iterációt használunk, annál nehezebb lesz feltörni a jelszót brute-force módszerrel, de annál több időbe telik a hash kiszámítása. Az ajánlott értékek általában 100 000 és 500 000 között vannak, de ez függ a rendszer teljesítményétől és a biztonsági követelményektől is. Ebben az esetben 150 000 iterációt választottam, ami jó egyensúlyt biztosít.
        public int Iterations => 150_000;

        private const int SaltSizeBytes = 16;
        private const int KeySizeBytes = 32;
        // A HashPassword metódus egy jelszót vesz be, és visszaadja a hash-t és a salt-ot Base64 formátumban. Először létrehoz egy véletlenszerű salt-ot a megadott méretben, majd kiszámolja a hash-t a PBKDF2 algoritmussal, amely iterációkat használ a biztonság növelése érdekében. Végül visszaadja a hash-t és a salt-ot Base64-ben kódolva, hogy könnyen tárolható legyen az adatbázisban.
        public (string HashBase64, string SaltBase64) HashPassword(string password)
        {// A jelszó nem lehet üres vagy csak whitespace, mert az gyenge jelszó lenne, és nem lenne értelme hash-elni. Ezért ellenőrizzük ezt a feltételt, és ha nem teljesül, akkor kivételt dobunk.
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("A jelszó nem lehet üres.", nameof(password));

            byte[] salt = RandomNumberGenerator.GetBytes(SaltSizeBytes);
            // A PBKDF2 algoritmus egy kulcsderivációs függvény, amely a jelszóból és a salt-ból egy hash-t generál. Az iterációk száma növeli a hash kiszámításának idejét, így megnehezíti a brute-force támadásokat. A hash algoritmus SHA256, és a kimeneti hossz 32 byte (256 bit), ami elég erős a jelszó tárolásához.
            byte[] key = Rfc2898DeriveBytes.Pbkdf2(
                password: password,
                salt: salt,
                iterations: Iterations,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: KeySizeBytes
            );

            return (Convert.ToBase64String(key), Convert.ToBase64String(salt));
        }
        // A VerifyPassword metódus egy jelszót, egy tárolt hash-t, egy tárolt salt-ot és az iterációk számát vesz be, és visszaadja, hogy a jelszó helyes-e. Először ellenőrzi, hogy a jelszó nem üres vagy csak whitespace, majd megpróbálja dekódolni a tárolt hash-t és salt-ot Base64 formátumban. Ha ez nem sikerül, akkor hamis értéket ad vissza. Ezután kiszámolja a hash-t a megadott jelszóval, salt-tal és iterációkkal ugyanazzal a PBKDF2 algoritmussal, mint a HashPassword metódusban. Végül összehasonlítja a számított hash-t a tárolt hash-sel fix időben, hogy elkerülje az időzítéses támadásokat.
        public bool VerifyPassword(string password, string storedHashBase64, string storedSaltBase64, int iterations)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;

            byte[] salt, storedHash;
            try
            {
                salt = Convert.FromBase64String(storedSaltBase64);
                storedHash = Convert.FromBase64String(storedHashBase64);
            }
            catch { return false; }
            // A PBKDF2 algoritmus ugyanaz, mint a HashPassword metódusban, de a bemeneti értékek a tárolt hash és salt alapján vannak megadva. Az iterációk száma is ugyanaz kell legyen, hogy a hash-ek összehasonlíthatóak legyenek.
            byte[] computed = Rfc2898DeriveBytes.Pbkdf2(
                password: password,
                salt: salt,
                iterations: iterations,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: storedHash.Length
            );
            // A CryptographicOperations.FixedTimeEquals metódus egy biztonságos módja annak, hogy két byte tömböt összehasonlítsunk anélkül, hogy az időzítés alapján következtetéseket lehetne levonni a hash értékekről. Ez megakadályozza az időzítéses támadásokat, ahol a támadó megpróbálja kitalálni a hash értékét az alapján, hogy mennyi időbe telik a hash összehasonlítása.
            return CryptographicOperations.FixedTimeEquals(computed, storedHash);
        }
    }
}

