using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Projekt_Backend.Models;
using Projekt_Backend.Services.Interfaces;
// Ez a szolgáltatás felelős az email küldéséért a rendszerben. Az EmailService osztály implementálja az IEmailService interfészt, és a konstruktorában az EmailSettings konfigurációs értékeket veszi át. A SendEmailAsync metódus létrehoz egy MailMessage objektumot a megadott címzettel, tárggyal és tartalommal, majd egy SmtpClient segítségével elküldi az emailt a konfigurált SMTP szerverre. A hitelesítési adatok és a szerver beállításai az EmailSettings-ben vannak megadva, így könnyen módosíthatók anélkül, hogy a kódot újra kellene írni. Ez a szolgáltatás lehetővé teszi, hogy a rendszer különböző helyein (pl. regisztráció, jelszó visszaállítás) egyszerűen küldjünk email értesítéseket a felhasználóknak.
namespace Projekt_Backend.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var message = new MailMessage();
            message.From = new MailAddress(_settings.From);
            message.To.Add(to);
            message.Subject = subject;
            message.Body = body;

            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.User, _settings.Password),
                EnableSsl = true
            };

            await client.SendMailAsync(message);
        }
    }
}