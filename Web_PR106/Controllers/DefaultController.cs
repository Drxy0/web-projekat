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

			var requestUri = Request.RequestUri;
			return Redirect(requestUri.AbsoluteUri + "Index.html");
		}

		[HttpGet]
		public IHttpActionResult Get()
		{
			return Ok(flights);
		}
	}
}
