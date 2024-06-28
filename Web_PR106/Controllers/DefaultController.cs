using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using System.IO;
using System.Xml;
using Web_PR106.Models;
using System.Diagnostics;
using static System.Runtime.CompilerServices.Unsafe;

namespace Web_PR106.Controllers
{
    public class DefaultController : ApiController
    {

		[HttpGet, Route("")]
		public RedirectResult Index()
		{
			var requestUri = Request.RequestUri;
			return Redirect(requestUri.AbsoluteUri + "Index.html");
		}

		[HttpGet]
		public IHttpActionResult Get()
		{
			return Ok(Global.ShownFlights.Where(x => x.IsDeleted == false));
		}

		public void Post([FromBody]SearchFilter searchFilter)
		{
			string startDestination = searchFilter.StartDestination;
			string endDestination = searchFilter.EndDestination;
			string airlinesName = searchFilter.AirlinesName;
			string departureDate, arrivalDate;

			if (searchFilter.DepartureDate != null) 
			{
				DateTime date = DateTime.Parse(searchFilter.DepartureDate);
				departureDate = date.ToString("dd/MM/yyyy");
			}
			else
			{
				departureDate = "01/01/0001";
			}
			if (searchFilter.ArrivalDate != null)
			{
				DateTime date = DateTime.Parse(searchFilter.DepartureDate);
				arrivalDate = date.ToString("dd/MM/yyyy");
			}
			else
			{
				arrivalDate = "01/01/0001";
			}

			//if form is left unfilled then reset
			if (string.IsNullOrEmpty(startDestination) &&
				string.IsNullOrEmpty(endDestination) &&
				string.IsNullOrEmpty(airlinesName) &&
				departureDate == "01/01/0001" &&
				arrivalDate == "01/01/0001")
			{
				Global.ShownFlights = new List<Flight>(Global.Flights);
				return;
			}

			if (!string.IsNullOrEmpty(startDestination))
			{
				foreach(Flight flight in Global.Flights)
				{
					if (flight.StartDestination != startDestination)
					{
						Global.ShownFlights.Remove(flight);
					}
				}
			}

			if (!string.IsNullOrEmpty(endDestination))
			{
				foreach (Flight flight in Global.Flights)
				{
					if (flight.EndDestination != endDestination)
					{
						Global.ShownFlights.Remove(flight);
					}
				}
			}
			if (!string.IsNullOrEmpty(airlinesName))
			{
				foreach (Flight flight in Global.Flights)
				{
					if (flight.Airline.Name != airlinesName)
					{
						Global.ShownFlights.Remove(flight);
					}
				}
			}

			if (departureDate != "01/01/0001")
			{
				foreach (Flight flight in Global.Flights)
				{
					Trace.WriteLine(flight.DepartureDateTime.Split(' ')[0]);
					if (flight.DepartureDateTime.Split(' ')[0] != departureDate)
					{
						Global.ShownFlights.Remove(flight);
					}
				}
			}

			if (arrivalDate != "01/01/0001")
			{
				foreach (Flight flight in Global.Flights)
				{
					if (flight.ArrivalDateTime.Split(' ')[0] != arrivalDate)
					{
						Global.ShownFlights.Remove(flight);
					}
				}
			}
		}
	}
}
