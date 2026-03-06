namespace Projekt_Backend.Services.Interfaces
{
    public interface IEmailService// Ez az interfész egy email küldő szolgáltatást definiál, amelynek egyetlen metódusa van: SendEmailAsync. Ez a metódus aszinkron módon küld egy emailt a megadott címre (to), a megadott tárggyal (subject) és tartalommal (body). Az implementációja felelős azért, hogy a megfelelő SMTP szerver beállításokkal és hitelesítéssel küldje el az emailt. Ez az interfész lehetővé teszi, hogy a különböző email szolgáltatók (pl. SMTP, SendGrid, Amazon SES) könnyen integrálhatók legyenek a rendszerbe anélkül, hogy a hívó kódnak tudnia kellene a konkrét implementáció részleteiről.
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
