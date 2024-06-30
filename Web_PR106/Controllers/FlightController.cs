using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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

			string departureDateTime = flightParams["departureDateTime"]?.ToString();
			DateTime date = DateTime.ParseExact(departureDateTime, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
			departureDateTime = date.ToString("dd/MM/yyyy HH:mm");

			string arrivalDateTime = flightParams["arrivalDateTime"]?.ToString();
			DateTime date2 = DateTime.ParseExact(arrivalDateTime, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
			arrivalDateTime = date2.ToString("dd/MM/yyyy HH:mm");

			Flight flight = new Flight()
			{
				Airline = new Airline()
				{
					Id = airlineId,
					Name = airline.Name,
				},
				Id = Global.SetFlightId(),
				StartDestination = flightParams["startDestination"]?.ToString(),
				EndDestination = flightParams["endDestination"]?.ToString(),
				DepartureDateTime = departureDateTime,
				ArrivalDateTime = arrivalDateTime,
				NumberOf_FreeSeats = flightParams["numberOf_FreeSeats"] != null ? Convert.ToInt32(flightParams["numberOf_FreeSeats"]) : 0,
				Price = flightParams["price"] != null ? Convert.ToDouble(flightParams["price"]) : 0,
				Status = FlightStatus.AKTIVAN
			};

			airline.ProvidedFlights.Add(flight);
			Global.SaveAirlineData();
			Global.LoadFlights();
			return Ok();
		}

		[HttpPost]
		[Route("edit")]
		public IHttpActionResult Edit([FromBody] JObject flightParams)
		{
			if (flightParams == null)
			{
				return BadRequest();
			}

			int airlineId = Convert.ToInt32(flightParams["airlineId"]);
			int flightId = Convert.ToInt32(flightParams["flightId"]);
			Airline airline = Global.Airlines.FirstOrDefault(x => x.Id == airlineId);
			Flight flight = airline.ProvidedFlights.FirstOrDefault(x => x.Id == flightId);


			string departureDateTime = flightParams["departureDateTime"]?.ToString();
			DateTime date = DateTime.ParseExact(departureDateTime, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
			departureDateTime = date.ToString("dd/MM/yyyy HH:mm");

			string arrivalDateTime = flightParams["arrivalDateTime"]?.ToString();
			DateTime date2 = DateTime.ParseExact(arrivalDateTime, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
			arrivalDateTime = date2.ToString("dd/MM/yyyy HH:mm");

			flight.DepartureDateTime = departureDateTime;
			flight.ArrivalDateTime = arrivalDateTime;

			int numberOfSeats = flightParams["numberOf_Seats"] != null ? Convert.ToInt32(flightParams["numberOf_Seats"]) : 0;
			int seatsToAdd = numberOfSeats - (flight.NumberOf_FreeSeats + flight.NumberOf_TakenSeats);

			if (flight.NumberOf_FreeSeats + seatsToAdd < 0)
			{
				return BadRequest("New number of seats cannot be so low that it trumps the number of available seats.");
			}
			else
			{
				flight.NumberOf_FreeSeats += seatsToAdd;
			}

			double price = flightParams["price"] != null ? Convert.ToDouble(flightParams["price"]) : 0;
			if (price != 0)
			{
				flight.Price = price;
			}

			Global.SaveAirlineData();
			return Ok();
		}


		[HttpPost]
		[Route("adminFilterFlights")]
		public IHttpActionResult AdminFilterFlights([FromBody] JObject searchFilter)
		{
			if (searchFilter == null || searchFilter.Count == 0)
			{
				return BadRequest();
			}

			List<Flight> filteredFlights = new List<Flight>(Global.Flights.Where(x => x.IsDeleted == false));

			string startDestination = searchFilter["startDestination"]?.ToString();
			string endDestination = searchFilter["endDestination"]?.ToString();
			string departureDate = searchFilter["departureDate"]?.ToString();
			if (string.IsNullOrEmpty(departureDate) || departureDate == "")
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
					if (!flight.StartDestination.Contains(startDestination))
					{
						filteredFlights.Remove(flight);
					}
				}
			}

			if (!string.IsNullOrEmpty(endDestination))
			{
				foreach (Flight flight in Global.Flights)
				{
					if (!flight.StartDestination.Contains(endDestination))
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
			var reservation = Global.Reservations.FirstOrDefault(u => u.Flight.Id == flightId);

			if (reservation != null && (reservation.Status == ReservationStatus.KREIRANA || reservation.Status == ReservationStatus.ODOBRENA))
			{
				return BadRequest("Cannot delete flight because there are active reservations.");
			}

			flight.IsDeleted = true;
			Global.SaveAirlineData();
			return Ok();
		}

		[HttpGet]
		[Route("getAll")]
		public IHttpActionResult GetAll()
		{
			return Ok(Global.Flights.Where(x => x.IsDeleted == false));
		}
	}
}
