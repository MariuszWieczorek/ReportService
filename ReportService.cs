using ReportService.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        /// <summary>
        /// Metoda, która będzie wywoływana co określony interval
        /// </summary>
        public ReportService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _timer.Elapsed  += DoWork;
            _timer.Start();
        }

        private void DoWork(object sender, ElapsedEventArgs e)
        {
            SendError();
            SendReport();
        }

        private void SendError()
        {
            var errors = _errorRepository.GetLastErrors(_intervalInMinutes);
            if (errors == null || !errors.Any())
                return;

            // TODO:Send Mail
            
        }

        private void SendReport()
        {
            var actualHour = DateTime.Now.Hour;

            if (actualHour < _sendHour)
                return;

        }


        protected override void OnStop()
        {
        }
    }
}
