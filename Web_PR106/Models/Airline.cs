using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_PR106.Models
{
	public class Airline
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public string ContactInfo { get; set; }
		public bool IsDeleted { get; set; }
		public List<Flight> ProvidedFlights { get; set; }
		public List<Review> Reviews { get; set; }
		public Airline() 
		{
			ProvidedFlights = new List<Flight>();
			Reviews = new List<Review>();
		}

		public Airline(string name, string address, string contactInfo)
		{
			Name = name;
			Address = address;
			ContactInfo = contactInfo;
		}
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