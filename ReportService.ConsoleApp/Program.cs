using Cipher;
using EmailSender;
using ReportService.Core.Domains;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.ConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
        {

            string emailReciver = @"m.wieczorek1972@gmail.com";
            int intervalInMinutes = 10;
            var htmlEmail = new GenerateHtmlEmail();

            var email = new Email(new EmailParams
            {
                HostSmtp = ConfigurationManager.AppSettings["HostSmtp"],
                Port = int.Parse(ConfigurationManager.AppSettings["Port"]),
                EnableSsl = bool.Parse(ConfigurationManager.AppSettings["EnableSsl"]),
                SenderName = ConfigurationManager.AppSettings["SenderName"],
                SenderEmail = ConfigurationManager.AppSettings["SenderEmail"],
                SenderEmailPassword = DecryptSenderEmailPassword(),
            });


            // pobieranie z bazy ostatniego raportu
            var report = new Report()
            {
                Id = 1,
                Title = "R/1/2020",
                Date = new DateTime(2020, 1, 1, 12, 0, 0),
                Positions = new List<ReportPosition>()
                {
                   new ReportPosition
                   {
                       Id = 1,
                       ReportId = 1,
                       Title = "Position 1",
                       Description = "Description 1",
                       Value = 43.01M
                   },
                   new ReportPosition
                   {
                       Id = 1,
                       ReportId = 1,
                       Title = "Position 2",
                       Description = "Description 2",
                       Value = 4311M
                   },
                   new ReportPosition
                   {
                       Id = 1,
                       ReportId = 1,
                       Title = "Position 3",
                       Description = "Description 3",
                       Value = 53.11M
                   },
                }

            };

            // pobranie listy błędów
            var errors = new List<Error>
            {
                new Error {Message = "błąd testowy 1", Date = DateTime.Now},
                new Error {Message = "błąd testowy 2", Date = DateTime.Now},
            };


            Console.WriteLine("Wysyłanie raportu dziennego ...");
            email.Send("Raport dzienny", htmlEmail.GenerateReport(report), emailReciver).Wait();
            Console.WriteLine("Wysyłano raport dzienny");


            Console.WriteLine("wysyłanie raportu błędów ...");
            // dodanie .Wait() pozwala na wywołanie metody asynchronicznej w nie asynchronicznej klasie
            // w aplikacji nie testowej tego nie używać !
            // ponieważ mogą powstać rówżne, trudne do zdiagnozowania błędy
            email.Send("Błędy w aplikacji", htmlEmail.GenerateErrors(errors, intervalInMinutes), emailReciver).Wait();
            Console.WriteLine("Wysyłano raport błędów");



        }

        private static string DecryptSenderEmailPassword()
        {
            var stringCipher = new StringCipher("163F0C86-673A-426F-97CA-2A60A44134C7");
            var encryptedPassword = ConfigurationManager.AppSettings["SenderEmailPassword"];
            if (encryptedPassword.StartsWith("encrypt:"))
            {
                var passwordToEncrypt = encryptedPassword.Replace("encrypt:", string.Empty);
                encryptedPassword = stringCipher.Encrypt(passwordToEncrypt);

                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                configFile.AppSettings.Settings["SenderEmailPassword"].Value = encryptedPassword;
                configFile.Save();
            }
            return stringCipher.Decrypt(encryptedPassword);
        }
    }
}
