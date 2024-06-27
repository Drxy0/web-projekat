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
	public class Reservation
	{
		public int Id { get; set; }
		public User User { get; set; }
		public Flight Flight { get; set;}
		public int NumberOfPassengers { get; set;}
		public double Price { get; set;}
		public ReservationStatus Status { get; set;}

		public Reservation() { }
	}
}