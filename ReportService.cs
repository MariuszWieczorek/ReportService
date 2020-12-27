using Cipher;
using EmailSender;
using ReportService.Core;
using ReportService.Core.Repositories;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ReportService
{
    public partial class ReportService : ServiceBase
    {
        private const int _sendHour = 8;
       
        private const int _intervalInMinutes = 30;
        private Timer _timer = new Timer(_intervalInMinutes * 1000); // ważne wybieramy namespace: System.Timers 
        
        private ErrorRepository _errorRepository = new ErrorRepository();
        private ReportRepository _reportRepository = new ReportRepository();
        private Email _email;
        private GenerateHtmlEmail _htmlEmail = new GenerateHtmlEmail();
        private string _receiverEmail;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private StringCipher _stringCipher = new StringCipher("x1");

        /// <summary>
        /// Metoda, która będzie wywoływana co określony interval
        /// </summary>
        public ReportService()
        {
            InitializeComponent();

            try
            {
                _receiverEmail = ConfigurationManager.AppSettings["ReceiverEmail"];
                _email = new Email(new EmailParams
                {
                    HostSmtp = ConfigurationManager.AppSettings["HostSmtp"],
                    Port = int.Parse(ConfigurationManager.AppSettings["Port"]),
                    EnableSsl  = bool.Parse(ConfigurationManager.AppSettings["HostSmtp"]), 
                    SenderName = ConfigurationManager.AppSettings["SenderName"],
                    SenderEmail = ConfigurationManager.AppSettings["SenderEmail"],
                    SenderEmailPassword = ConfigurationManager.AppSettings["SenderEmailPassword"],
                });
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        protected override void OnStart(string[] args)
        {
            _timer.Elapsed  += DoWork;
            _timer.Start();
            Logger.Info("Service Started ...");
        }

       // w tym przypadku musimy pozostawić void zamiast Task
       // bo nie będziemy mogli się podpiąć do _timer.Elapsed
        private async void DoWork(object sender, ElapsedEventArgs e)
        {

            try
            {
                // wskazówki odnośnie używania metod asynchronicznych wyjątek przy void zamiast Task
               // await SendError();
                await SendReport();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                throw new Exception(ex.Message);
            }
            
        }
 
        private async Task SendError()
        {
            var errors = _errorRepository.GetLastErrors(_intervalInMinutes);
            if (errors == null || !errors.Any())
                return;

            // TODO:Send Mail
            
            // wskazówki odnośnie używania metod asynchronicznych
            await _email.Send("Błędy w aplikacji", _htmlEmail.GenerateErrors(errors,_intervalInMinutes),_receiverEmail);
            Logger.Info("Error was sent ...");
        }

        private async Task SendReport()
        {
            var actualHour = DateTime.Now.Hour;

            if (actualHour < _sendHour)
                return;

            var report = _reportRepository.GetLastNotSentReport();

            if (report == null)
                return;

            // TODO:Send Mail
            await _email.Send("Raport dzienny", _htmlEmail.GenerateReport(report), _receiverEmail);

            _reportRepository.ReportSent(report);
            Logger.Info("Report was sent ...");
        }


        protected override void OnStop()
        {
            Logger.Info("Service Stopped ...");
        }
         
    }
}
