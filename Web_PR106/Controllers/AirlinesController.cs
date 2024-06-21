using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml;
using Web_PR106.Models;

namespace Web_PR106.Controllers
{
    public class AirlinesController : ApiController
    {
		private static List<Airline> airlines = new List<Airline>();
		private static bool loaded = false;
		public void LoadDatabase()
		{
			loaded = true;
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			string fullPath = baseDirectory + "\\Assets\\test_aviokompanije.xml";

			XmlDocument doc = new XmlDocument();
			doc.Load(fullPath);

			foreach (XmlNode node in doc.DocumentElement.ChildNodes)
			{
				Airline airline = new Airline()
				{
					Name = node["Name"].InnerText,
					Address = node["Address"].InnerText,
					ContactInfo = node["ContactInfo"].InnerText,
					ProvidedFlights = new List<Flight>(),
					Reviews = new List<Review>(),
				};


				XmlNode flightsNode = node["ProvidedFlights"];
				if (flightsNode != null)
				{
					foreach (XmlNode flightNode in flightsNode.ChildNodes)
					{
						Flight flight = new Flight()
						{
							Aviokompanija = new Airline(flightNode["Aviokompanija"].InnerText),
							StartDestination = flightNode["StartDestination"].InnerText,
							EndDestination = flightNode["EndDestination"].InnerText,
							DepartureDateTime = flightNode["DepartureDateTime"].InnerText,
							ArrivalDateTime = flightNode["ArrivalDateTime"].InnerText,
							NumberOf_FreeSeats = int.Parse(flightNode["NumberOf_FreeSeats"].InnerText),
							NumberOf_TakenSeats = int.Parse(flightNode["NumberOf_TakenSeats"].InnerText),
							Price = double.Parse(flightNode["Price"].InnerText),
							Status = (FlightStatus)Enum.Parse(typeof(FlightStatus), flightNode["Status"].InnerText)
						};
						airline.ProvidedFlights.Add(flight);
					}
				}

				XmlNode reviewsNode = node["Reviews"];
				if (reviewsNode != null)
				{
					foreach (XmlNode reviewNode in reviewsNode.ChildNodes)
					{
						Review review = new Review()
						{
							Reviewer = reviewNode["Reviewer"].InnerText,
							Airline = reviewNode["Aviokompanija"].InnerText,
							Title = reviewNode["Title"].InnerText,
							Description = reviewNode["Description"].InnerText,
							Picture = reviewNode["Picture"].InnerText,
							Status = (ReviewStatus)Enum.Parse(typeof(ReviewStatus), reviewNode["Status"].InnerText)
						};
						airline.Reviews.Add(review);
					}
				}

				airlines.Add(airline);
			}
		}

		[HttpGet]
        public IHttpActionResult Get()
        {
            if (!loaded)
            {
				LoadDatabase();
			}
            return Ok(airlines);
        }

    }
}
