using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Web_PR106.Models;

namespace Web_PR106.Controllers
{
    public class ReservationController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok(Global.Reservations);
        }

        [HttpPost]
        public IHttpActionResult Post([FromBody] JObject reservationData)
        {
            int reservationId = int.Parse(reservationData["reservationId"]?.ToString());
            string status = reservationData["reservationId"]?.ToString();
            string username = reservationData["username"]?.ToString();
            int numberOfPassengers = int.Parse(reservationData["numberOfPassengers"]?.ToString());
            int flightId = int.Parse(reservationData["flightId"]?.ToString());

            Reservation res = Global.Reservations.FirstOrDefault(x => x.Id == reservationId);

			DateTime dt = DateTime.ParseExact(res.Flight.DepartureDateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
			DateTime now = DateTime.Now;
			TimeSpan difference = dt - now;

            ReservationStatus resStatus = (ReservationStatus)Enum.Parse(typeof(ReservationStatus), status);

			if (difference.TotalHours < 24 && resStatus.Equals(ReservationStatus.OTKAZANA))
            {
                return BadRequest("Cannoct cancel reservation 24h before departure.");
            }

            res.Status = resStatus;

            User user = Global.Users.FirstOrDefault(u => u.Username == username);
            Reservation reservation =  user.ReservationList.FirstOrDefault(u => u.Id == reservationId);
            reservation.Status = resStatus;

			if (resStatus.Equals(ReservationStatus.OTKAZANA))
            {
                reservation.Flight.NumberOf_FreeSeats += numberOfPassengers;

                foreach(Airline airline in Global.Airlines)
                {
                    Flight flight = airline.ProvidedFlights.FirstOrDefault(u => u.Id == flightId);
                    flight.NumberOf_FreeSeats += numberOfPassengers;
                    flight.NumberOf_TakenSeats -= numberOfPassengers;
                }
            }

			return Ok();
        }
    }
}
