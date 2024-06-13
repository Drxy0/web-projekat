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
		public Aviokompanija(string name)
		{
			Name = name;
		}
		public Aviokompanija(string name, string address, string contactInfo, List<Let> providedFlights, List<Recenzija> reviews) : this(name)
		{
			Address = address;
			ContactInfo = contactInfo;
			ProvidedFlights = providedFlights;
			Reviews = reviews;
		}
	}
}