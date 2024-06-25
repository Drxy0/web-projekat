using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web_PR106.Models
{
	public class AdminSearchUsersFilter
	{
		public string Name { get; set; }
		public string Surname { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

		public AdminSearchUsersFilter() { }
	}
}