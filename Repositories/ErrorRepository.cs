﻿using ReportService.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.Repositories
{
    public class ErrorRepository
    {
        public List<Error> GetLastErrors(int intervalInMinutes)
        {
            // docelowo pobieranie z bazy

            return new List<Error>
            {
                new Error {Message = "błąd testowy 1", Date = DateTime.Now},
                new Error {Message = "błąd testowy 1", Date = DateTime.Now},
            };

        }
    }
}
