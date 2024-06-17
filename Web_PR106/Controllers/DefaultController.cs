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

		private static List<Let> flights = new List<Let>();
		private static List<Let> shownFlights = new List<Let>();

		[HttpGet, Route("")]
		public RedirectResult Index()
		{
			string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			string fullPath = baseDirectory + "\\Assets\\test_letovi.xml";

			XmlDocument doc = new XmlDocument();
			doc.Load(fullPath);

			foreach (XmlNode node in doc.DocumentElement.ChildNodes)
			{
				Let flight = new Let();

				flight.Aviokompanija = new Aviokompanija(node["Aviokompanija"].InnerText);
				flight.StartDestination = node["StartDestination"].InnerText;
				flight.EndDestination = node["EndDestination"].InnerText;
				flight.DepartureDateTime = node["DepartureDateTime"].InnerText;
				flight.ArrivalDateTime = node["ArrivalDateTime"].InnerText;
				flight.NumberOf_FreeSeats = int.Parse(node["NumberOf_FreeSeats"].InnerText);
				flight.NumberOf_TakenSeats = int.Parse(node["NumberOf_TakenSeats"].InnerText);
				flight.Price = double.Parse(node["Price"].InnerText);
				flight.Status = (FlightStatus)Enum.Parse(typeof(FlightStatus), node["Status"].InnerText);

				flights.Add(flight);
			}
			shownFlights = new List<Let>(flights);

			var requestUri = Request.RequestUri;
			return Redirect(requestUri.AbsoluteUri + "Index.html");
		}

		[HttpGet]
		public IHttpActionResult Get()
		{
			return Ok(shownFlights);
		}

		public void Post([FromBody]SearchFilter searchFilter)
		{

			string startDestination = searchFilter.StartDestination;
			string endDestination = searchFilter.EndDestination;
			string airlinesName = searchFilter.AirlinesName;
			string departureDate, arrivalDate;

			if (searchFilter.DepartureDate != null) { 
				departureDate = searchFilter.DepartureDate.ToString().Split(' ')[0];
			}
			else
			{
				departureDate = "01/01/0001";
			}
			if (searchFilter.ArrivalDate != null)
			{
				arrivalDate = searchFilter.ArrivalDate.ToString().Split(' ')[0];
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
				shownFlights = new List<Let>(flights);
				return;
			}

			if (!string.IsNullOrEmpty(startDestination))
			{
				foreach(Let flight in flights)
				{
					if (flight.StartDestination != startDestination)
					{
						shownFlights.Remove(flight);
					}
				}
			}

			if (!string.IsNullOrEmpty(endDestination))
			{
				foreach (Let flight in flights)
				{
					if (flight.EndDestination != endDestination)
					{
						shownFlights.Remove(flight);
					}
				}
			}
			if (!string.IsNullOrEmpty(airlinesName))
			{
				foreach (Let flight in flights)
				{
					if (flight.Aviokompanija.Name != airlinesName)
					{
						shownFlights.Remove(flight);
					}
				}
			}
			if (departureDate != "01/01/0001")
			{
				foreach (Let flight in flights)
				{
					if (flight.DepartureDateTime != departureDate)
					{
						shownFlights.Remove(flight);
					}
				}
			}

			if (arrivalDate != "01/01/0001")
			{
				foreach (Let flight in flights)
				{
					if (flight.ArrivalDateTime != arrivalDate)
					{
						shownFlights.Remove(flight);
					}
				}
			}
		}
	}
}
