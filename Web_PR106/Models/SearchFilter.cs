using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_PR106.Models
{
	public class SearchFilter
	{
		public string StartDestination { get; set; }
		public string EndDestination { get; set; }
		public string AirlinesName { get; set; }
		public string DepartureDate { get; set; }
		public string ArrivalDate { get; set; }

		public SearchFilter() { }
	}
}