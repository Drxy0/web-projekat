using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_PR106.Models
{
	public enum ReviewStatus
	{
		KREIRANA,
		ODOBRENA,
		ODBIJENA
	}
	public class Recenzija
	{
		public Korisnik Reviewer;
		public Aviokompanija Airline;
		public string Title;
		public string Description;
		public string Picture;
		public ReviewStatus Status;

		public Recenzija() { }
	}
}