using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_PR106.Models
{
    public class DataModel
    {
        public Airline Aviokompanija { get; set; }
        public User Korisnik { get; set; }
        public List<Flight> Letovi { get; set; }
        public Review Recenzija { get; set; }
        public Reservation Rezervacija { get; set; }
    }
}