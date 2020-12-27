using EmailSender;
using ReportService.Core.Domains;
using System;
using System.Collections.Generic;
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

            //  465,587
            // "txoisgkslphjeogp" "rmhfvaurzyxnuztn"
            var email = new Email(new EmailParams
            {
                HostSmtp = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                SenderName = "Mariusz Wieczorek",
                SenderEmail = "mariusz.wieczorek.testy@gmail.com",
                SenderEmailPassword = "rmhfvaurzyxnuztn" 
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
    }
}
