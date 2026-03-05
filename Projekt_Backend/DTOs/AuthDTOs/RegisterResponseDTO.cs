namespace Projekt_Backend.DTOs.AuthDTOs
{// Ez a DTO a regisztrációs válaszok adatainak reprezentációjára szolgál. A regisztráció sikeres végrehajtása után a controller egy RegisterResponseDto objektumot ad vissza, amely tartalmazza az újonnan létrehozott felhasználó azonosítóját (ClientId) és email címét. Ez lehetővé teszi a kliens számára, hogy megkapja a létrehozott felhasználó alapvető adatait anélkül, hogy érzékeny információkat (például jelszó hash vagy salt) tartalmazna.
    public sealed class RegisterResponseDto
    {
        public int ClientId { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
