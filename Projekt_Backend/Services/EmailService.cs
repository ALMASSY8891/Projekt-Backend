using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Projekt_Backend.Models;
using Projekt_Backend.Services.Interfaces;

namespace Projekt_Backend.Services
{
    // Ez az osztály felelős az email küldéséért a megadott beállítások alapján. Az IEmailService interfészt valósítja meg, amely meghatározza a SendEmailAsync metódust.
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body) // Ez a metódus felelős az email küldéséért. Először lefordítja a státuszokat magyarra, majd létrehoz egy MailMessage objektumot a megadott adatokkal, és végül elküldi az emailt egy SmtpClient segítségével.
        {
            var translatedBody = TranslateStatuses(body);

            var message = new MailMessage
            {
                From = new MailAddress(_settings.From),
                Subject = subject,
                Body = translatedBody,
                IsBodyHtml = false
            };

            message.To.Add(to);

            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.User, _settings.Password),
                EnableSsl = true
            };

            await client.SendMailAsync(message);
        }

        private static string TranslateStatuses(string body) // Ez a segédfüggvény felelős a státuszok magyarra fordításáért. A Regex.Replace metódus segítségével cseréli le az angol státuszokat a magyar megfelelőikre a megadott szövegben.
        {
            if (string.IsNullOrWhiteSpace(body))
                return body;

            body = Regex.Replace(body, @"\bNew\b", "Új");
            body = Regex.Replace(body, @"\bPending\b", "Folyamatban");
            body = Regex.Replace(body, @"\bConfirmed\b", "Jóváhagyva");
            body = Regex.Replace(body, @"\bCompleted\b", "Teljesítve");
            body = Regex.Replace(body, @"\bCancelled\b", "Törölve");

            return body;
        }
    }
}