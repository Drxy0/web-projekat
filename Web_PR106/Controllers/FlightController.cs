using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml.Linq;
using Web_PR106.Models;

namespace Web_PR106.Controllers
{
	[RoutePrefix("api/flight")]
	public class FlightController : ApiController
    {
		[HttpPost]
		[Route("adminFilterFlights")]
		public IHttpActionResult AdminFilterFlights([FromBody] JObject searchFilter)
		{
			if (searchFilter == null)
			{
				return BadRequest();
			}

			List<Flight> filteredFlights = new List<Flight>(Global.Flights);

			string startDestination = searchFilter["startDestination"]?.ToString(); // Accessing "name" property from JSON
			string endDestination = searchFilter["endDestination"]?.ToString(); // Accessing "address" property from JSON
			string departureDate = searchFilter["departureDate"]?.ToString(); // Accessing "contactInfo" property from JSON

			if (string.IsNullOrEmpty(startDestination) &&
				string.IsNullOrEmpty(endDestination) &&
				string.IsNullOrEmpty(departureDate))
			{
				return Ok(Global.Flights);
			}


			if (!string.IsNullOrEmpty(startDestination))
			{
				foreach (Flight flight in Global.Flights)
				{
					if (flight.StartDestination != startDestination)
					{
						filteredFlights.Remove(flight);
					}
				}
			}

			if (!string.IsNullOrEmpty(endDestination))
			{
				foreach (Flight flight in Global.Flights)
				{
					if (flight.EndDestination != endDestination)
					{
						filteredFlights.Remove(flight);
					}
				}
			}

			if (!string.IsNullOrEmpty(departureDate))
			{
				foreach (Flight flight in Global.Flights)
				{
					if (flight.DepartureDateTime != departureDate)
					{
						filteredFlights.Remove(flight);
					}
				}
			}

			return Ok(filteredFlights);
		}


		[HttpDelete]
		[Route("delete/{airlineId}/flight/{flightId}")]
		public IHttpActionResult Delete(int airlineId, int flightId)
		{
			var airline = Global.Airlines.FirstOrDefault(u => u.Id == airlineId);
			if (airline == null)
			{
				return NotFound();
			}
			var flight = airline.ProvidedFlights.FirstOrDefault(u => u.Id == flightId);
			if (flight == null)
			{
				return NotFound();
			}

			flight.IsDeleted = true;
			return Ok();
		}
	}
}
