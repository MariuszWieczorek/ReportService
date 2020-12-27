using ReportService.Core.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService
{
    public class GenerateHtmlEmail
    {
        public string GenerateErrors(List<Error> errors, int interval)
        {
            if (errors != null)
                throw new ArgumentNullException(nameof(errors));

            if (!errors.Any())
                return String.Empty;

            var html = $"Błędy z ostatnich {interval} minut. <br /> <br />";
            html += $@"<table border=1 cellpadding=5  cellspacing=1>
                <tr>
                    <td> align=center bgcolor=lightgrey>Wiadomość</td>
                    <td> align=center bgcolor=lightgrey>data</td>
                    
                </tr>
                ";
            
            foreach(var error in errors)
            {
                html +=
                $@"<tr>
                    <td> align=center bgcolor=lightgrey>{error.Message}</td>
                    <td> align=center bgcolor=lightgrey>{error.Date.ToString("dd-MM-yyyy HH:mm")}</td>
                </tr>
                ";
            }

            html += $@"</table> <br /> <br /> <i>Automatyczna wiadomość wysłana z aplikacji </i>";

            return html;
        }

        public string GenerateReport(Report report)
        {
            if (report==null)
                throw new ArgumentNullException(nameof(report));


            var html = $@"Raport {report.Title} 
                        z dnia {report.Date.ToString("dd-MM-YYYY")}. <br /> <br />";


            // Jeżeli raport zawiera jakiekolwiek pozycje to generujemy tabelkę w HTML
            if (report.Positions != null && report.Positions.Any())
            {
                html += $@"<table border=1 cellpadding=5  cellspacing=1>
                <tr>
                    <td> align=center bgcolor=lightgrey>Tytuł</td>
                    <td> align=center bgcolor=lightgrey>Opis</td>
                    <td> align=center bgcolor=lightgrey>Wartość</td>
                </tr>
                ";

                foreach (var position in report.Positions)
                {
                    html +=
                    $@"<tr>
                    <td> align=center bgcolor=lightgrey>{position.Title}</td>
                    <td> align=center bgcolor=lightgrey>{position.Description}</td>
                    <td> align=center bgcolor=lightgrey>{position.Value.ToString("0.00")} zł</td>
                </tr>
                ";
                }

                html += $@"</table>";
            }
            else
                html += $@"-- brak danych do wyświetlenia --";

            html += $@"<br/><br/> <i>Automatyczna wiadomość wysłana z aplikacji </i>";

            return html;
        }
    }
}
