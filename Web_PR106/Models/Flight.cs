using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_PR106.Models
{
	public enum FlightStatus
	{
		AKTIVAN,
		OTKAZAN,
		ZAVRSEN
	}
	public class Flight
	{
		public int Id { get; set; }
		public Airline Airline { get; set; }
		public string StartDestination { get; set; }
		public string EndDestination { get; set; }
		public string DepartureDateTime { get; set; }
		public string ArrivalDateTime { get; set; }
		public int NumberOf_FreeSeats { get; set; }
		public int NumberOf_TakenSeats { get; set; }
		public double Price { get; set; }
		public FlightStatus Status { get; set; }
		public bool IsDeleted { get; set; }

		public Flight () { }
	}
}