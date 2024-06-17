using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_PR106.Models
{
	public class Airline
	{
		public string Name { get; set; }
		public string Address { get; set; }
		public string ContactInfo { get; set; }
		public List<Flight> ProvidedFlights { get; set; }
		public List<Review> Reviews { get; set; }
		public Airline() { }
		public Airline(string name)
		{
			Name = name;
		}
		public Airline(string name, string address, string contactInfo, List<Flight> providedFlights, List<Review> reviews) : this(name)
		{
			Address = address;
			ContactInfo = contactInfo;
			ProvidedFlights = providedFlights;
			Reviews = reviews;
		}
	}
}