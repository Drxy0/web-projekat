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
		public IHttpActionResult Post([FromBody] JObject flightParams)
		{
			if (flightParams == null) 
			{ 
				return BadRequest(); 
			}

			int airlineId = Convert.ToInt32(flightParams["airline"]);
			Airline airline = Global.Airlines.FirstOrDefault(x => x.Id == airlineId);

			Flight flight = new Flight()
			{
				Airline = new Airline()
				{
					Id = airlineId,
					Name = airline.Name,
				},
				//Id =
				StartDestination = flightParams["startDestination"]?.ToString(),
				EndDestination = flightParams["endDestination"]?.ToString(),
				DepartureDateTime = flightParams["departureDateTime"]?.ToString(),
				ArrivalDateTime = flightParams["arrivalDateTime"]?.ToString(),
				NumberOf_FreeSeats = flightParams["numberOf_FreeSeats"] != null ? Convert.ToInt32(flightParams["numberOf_FreeSeats"]) : 0,
				Price = flightParams["price"] != null ? Convert.ToDouble(flightParams["price"]) : 0,
				Status = FlightStatus.AKTIVAN
			};

			airline.ProvidedFlights.Add(flight);

			Global.LoadFlights();
			return Ok();
		}


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
			string departureDate = searchFilter["departureDate"]?.ToString();
			if (!string.IsNullOrEmpty(departureDate) || departureDate == "")
			{
				departureDate = "01/01/0001";
			}
			else
			{
				DateTime date = DateTime.Parse(departureDate);
				departureDate = date.ToString("dd/MM/yyyy");
			}

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

			if (departureDate != "01/01/0001")
			{
				foreach (Flight flight in Global.Flights)
				{
					if (flight.DepartureDateTime.Split(' ')[0] != departureDate)
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
