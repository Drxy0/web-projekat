using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_PR106.Models
{
    public class DataModel
    {
        public Aviokompanija Aviokompanija { get; set; }
        public Korisnik Korisnik { get; set; }
        public List<Let> Letovi { get; set; }
        public Recenzija Recenzija { get; set; }
        public Rezervacija Rezervacija { get; set; }
    }
}