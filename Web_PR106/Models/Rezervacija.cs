using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_PR106.Models
{
	public enum ReservationStatus
	{
		KREIRANA,
		ODOBRENA,
		OTKAZANA,
		ZAVRSENA,
	}
	public class Rezervacija
	{
		public Korisnik User;
		public Let Flight;
		public int NumberOfPassangers;
		public double Price;
		public ReservationStatus Status;

		public Rezervacija() { }
	}
}