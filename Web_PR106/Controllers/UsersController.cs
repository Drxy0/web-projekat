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
			int flightId = int.Parse(request.Split(' ')[0]);
			int numberOfPassangers = int.Parse(request.Split(' ')[1]);
			string username = request.Split(' ')[2];
			if (numberOfPassangers.Equals(null))
			{
				return BadRequest();
			}

			Flight selectedFlight = Global.Flights.Find(x => x.FlightId == flightId);
			selectedFlight.NumberOf_FreeSeats -= numberOfPassangers;
			selectedFlight.NumberOf_TakenSeats += numberOfPassangers;

			List<Reservation> reservationList = Global.Users.Find(x => x.Username == username).ReservationList;
			Reservation res = new Reservation()
			{
				User = new User() { Username = username },
				Flight = selectedFlight,
				NumberOfPassangers = numberOfPassangers,
				Price = selectedFlight.Price * numberOfPassangers,
				Status = ReservationStatus.KREIRANA
			};
			reservationList.Add(res);
			return Ok();
		}
	}
}
