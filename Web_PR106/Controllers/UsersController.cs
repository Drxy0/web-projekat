using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml;
using Web_PR106.Models;

namespace Web_PR106.Controllers
{
	[RoutePrefix("api/users")]
	public class UsersController : ApiController
    {

		[HttpGet]
        public IHttpActionResult Get()
        {
			return Ok(Global.Users);
		}

		[HttpPost]
		public IHttpActionResult Post([FromBody]User user)
        {
			User oldUser = Global.Users.Find(x => x.Username == user.Username);

			if(oldUser != null)		//Replace already existing user info
			{
				Global.Users.Remove(oldUser);
			}
			Global.Users.Add(user);
			return Ok();
        }

		[HttpPost]
		[Route("adminFilterUsers")]
		public IHttpActionResult AdminFilterUsers([FromBody]AdminSearchUsersFilter searchFilter)
		{
			List<User> filteredUsers = new List<User>(Global.Users);
			string startDate, endDate;

			if (searchFilter.StartDate != null)
			{
				startDate = searchFilter.StartDate.ToString().Split(' ')[0];
			}
			else
			{
				startDate = "01/01/0001";
			}
			if (searchFilter.EndDate != null)
			{
				endDate = searchFilter.EndDate.ToString().Split(' ')[0];
			}
			else
			{
				endDate = "01/01/0001";
			}

			//if form is left unfilled then reset
			if (string.IsNullOrEmpty(searchFilter.Name) &&
				string.IsNullOrEmpty(searchFilter.Surname) &&
				startDate == "01/01/0001" &&
				endDate == "01/01/0001")
			{
				return Ok(Global.Users);
			}

			if (!string.IsNullOrEmpty(searchFilter.Name))
			{
				foreach (User user in Global.Users)
				{
					if (user.Name != searchFilter.Name)
					{
						filteredUsers.Remove(user);
					}
				}
			}

			if (!string.IsNullOrEmpty(searchFilter.Surname))
			{
				foreach (User user in Global.Users)
				{
					if (user.Surname != searchFilter.Surname)
					{
						filteredUsers.Remove(user);
					}
				}
			}

			if (startDate != "01/01/0001")
			{
				DateTime startdt = DateTime.ParseExact(startDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
				foreach (User user in Global.Users)
				{
					DateTime bday= DateTime.ParseExact(user.Birthday, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
					if (bday < startdt)//
					{
						filteredUsers.Remove(user);
					}
				}
			}

			if (startDate != "01/01/0001")
			{
				DateTime enddt = DateTime.ParseExact(startDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
				foreach (User user in Global.Users)
				{
					DateTime bday= DateTime.ParseExact(user.Birthday, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
					if (bday > enddt)
					{
						filteredUsers.Remove(user);
					}
				}
			}

			return Ok(filteredUsers);
		}


		[HttpPost]
		[Route("filterFlightsByStatus")]
		public IHttpActionResult FilterFlightsByStatus([FromBody] string request)
		{
			string status = request.Split(' ')[0];
			string username = request.Split(' ')[1];

			if (status == "")
			{
				return BadRequest();
			}

			FlightStatus flightStatus = (FlightStatus)Enum.Parse(typeof(FlightStatus), status);

			User currentUser = Global.Users.Find(x => x.Username == username);

			var filteredFlights = currentUser.ReservationList
				.Where(r => r.Flight.Status == flightStatus)
				.Select(r => r.Flight)
				.ToList();
			return Ok(filteredFlights);
		}


		[HttpPost]
		[Route("filterReservationsByStatus")]
		public IHttpActionResult FilterReservationsByStatus([FromBody] string request)
		{
			string status = request.Split(' ')[0];
			string username = request.Split(' ')[1];
			if (status == "")
			{
				return BadRequest();
			}

			ReservationStatus reservationStatus = (ReservationStatus)Enum.Parse(typeof(ReservationStatus), status);

			User currentUser = Global.Users.Find(x => x.Username == username);

			var filteredReservations = currentUser.ReservationList
				.Where(r => r.Status == reservationStatus)
				.ToList();
			return Ok(filteredReservations);
		}

		[HttpPost]
		[Route("createReservation")]
		public IHttpActionResult CreateReservation([FromBody] string request)
		{
			try
			{
				int flightId = int.Parse(request.Split(' ')[0]);
				int numberOfPassengers = int.Parse(request.Split(' ')[1]);
				string username = request.Split(' ')[2];

				if (numberOfPassengers <= 0)
				{
					return BadRequest("Number of passengers must be greater than zero.");
				}

				// Check if a reservation already exists for the given flight and username
				List<Reservation> reservationList = Global.Users.Find(x => x.Username == username)?.ReservationList;
				Reservation existingReservation = reservationList?.Find(x => x.Flight.Id == flightId);

				if (existingReservation != null)
				{
					if (existingReservation.Status == ReservationStatus.KREIRANA || existingReservation.Status == ReservationStatus.ODOBRENA)
					{
						// Cancel the existing reservation
						CancelExistingReservation(existingReservation);
					}
					else
					{
						return BadRequest("Invalid reservation status.");
					}
				}

				// Proceed with creating the new reservation
				Flight selectedFlight = Global.Flights.Find(x => x.Id == flightId);
				if (selectedFlight == null)
				{
					return NotFound(); // Flight not found
				}

				if (numberOfPassengers > selectedFlight.NumberOf_FreeSeats)
				{
					return BadRequest("Number of passengers exceeds available seats.");
				}

				// Update seat counts
				selectedFlight.NumberOf_FreeSeats -= numberOfPassengers;
				selectedFlight.NumberOf_TakenSeats += numberOfPassengers;

				// Create new reservation
				Reservation newReservation = new Reservation()
				{
					User = new User() { Username = username },
					Flight = selectedFlight,
					NumberOfPassengers = numberOfPassengers,
					Price = selectedFlight.Price * numberOfPassengers,
					Status = ReservationStatus.KREIRANA
				};

				reservationList.Add(newReservation);

				return Ok("Reservation created successfully.");
			}
			catch (Exception ex)
			{
				Trace.WriteLine($"Exception occurred: {ex.Message}");
				return BadRequest("Failed to create reservation.");
			}
		}

		private void CancelExistingReservation(Reservation existingReservation)
		{
			Flight selectedFlight = existingReservation.Flight;
			selectedFlight.NumberOf_FreeSeats += existingReservation.NumberOfPassengers;
			selectedFlight.NumberOf_TakenSeats -= existingReservation.NumberOfPassengers;

			Global.Users
				.Find(x => x.Username == existingReservation.User.Username)?
				.ReservationList
				.Remove(existingReservation);
		}


		[HttpPost]
		[Route("cancelReservation")]
		public IHttpActionResult CancelReservation([FromBody] string request)
		{
			try
			{
				int flightId = int.Parse(request.Split(' ')[0]);
				string username = request.Split(' ')[1];

				Flight selectedFlight = Global.Flights.Find(x => x.Id == flightId);
				if (selectedFlight == null)
				{
					return NotFound(); // Flight not found
				}

				string[] dateTime = selectedFlight.DepartureDateTime.Split('T');
				string[] dateStr = dateTime[0].Split('-');
				string[] timeStr = dateTime[1].Split(':');

				int[] date = dateStr.Select(int.Parse).ToArray();
				int[] time = timeStr.Select(int.Parse).ToArray();

				DateTime dtFlight = new DateTime(date[0], date[1], date[2], time[0], time[1], time[2]);
				DateTime now = DateTime.Now;

				TimeSpan difference = dtFlight - now;

				if (difference.TotalHours < 24)
				{
					return StatusCode(System.Net.HttpStatusCode.PreconditionFailed); // Less than 24 hours before flight
				}

				var user = Global.Users.Find(x => x.Username == username);
				if (user == null)
				{
					return NotFound(); // User not found
				}

				var reservationList = user.ReservationList;
				Reservation foundReservation = reservationList.Find(x => x.Flight.Id == flightId);

				if (foundReservation == null)
				{
					return NotFound(); // Reservation not found
				}

				selectedFlight.NumberOf_FreeSeats += foundReservation.NumberOfPassengers;
				selectedFlight.NumberOf_TakenSeats -= foundReservation.NumberOfPassengers;

				reservationList.Remove(foundReservation);

				return Ok();
			}
			catch (Exception ex)
			{
				Trace.WriteLine($"Exception occurred: {ex.Message}");
				return BadRequest();
			}
		}

		[HttpPost]
		[Route("reservationExists")]
		public IHttpActionResult ReservationExists([FromBody] string request)
		{
			int flightId = int.Parse(request.Split(' ')[0]);
			string username = request.Split(' ')[1];

			List<Reservation> reservationList = Global.Users.Find(x => x.Username == username).ReservationList;
			Reservation foundReservation = reservationList.Find(x => x.Flight.Id == flightId);
			if (foundReservation != null)
			{
				return Ok(true);
			}
			else
			{
				return Ok(false);
			}
		}

	}
}
