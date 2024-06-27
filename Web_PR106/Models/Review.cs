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
	public class Review
	{
		public int Id { get; set; }
		public string Reviewer { get; set; }
		public string Airline { get; set;}
		public string Title { get; set;}
		public string Description { get; set;}
		public string Picture { get; set;}
		public ReviewStatus Status { get; set;}

		public Review() { }
	}
}