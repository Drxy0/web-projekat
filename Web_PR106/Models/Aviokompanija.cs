using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_PR106.Models
{
	public class Aviokompanija
	{
		public string Name;
		public string Address;
		public string ContactInfo;
		public List<Let> ProvidedFlights;
		public List<Recenzija> Reviews;

		public Aviokompanija() { }
	}
}