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
	public class Let
	{
		public Aviokompanija Aviokompanija;
		public string StartDestination;
		public string EndDestination;
		public string DepartureDateTime;
		public string ArrivalDateTime;
		public int NumberOf_FreeSeats;
		public int NumberOf_TakenSeats;
		public double Price;
		public FlightStatus Status;

		public Let () { }
	}
}