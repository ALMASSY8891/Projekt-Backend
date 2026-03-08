namespace Projekt_Backend.Services.Interfaces
{
    // Az ITokenService interfész egy szolgáltatás szerződését határozza meg, amely felelős a JWT tokenek létrehozásáért. Ez az interfész lehetővé teszi a token létrehozását a megadott paraméterek alapján, mint például a kliens azonosítója, email címe, neve és szerepköre. A konkrét implementációban ez a szolgáltatás valószínűleg a JWT tokenek generálásáért és aláírásáért felelős logikát fogja tartalmazni.
    public interface ITokenService
    {
        string CreateToken(int clientId, string email, string name, string role);
    }
}

