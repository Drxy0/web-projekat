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
        private static List<User> users = new List<User>();
		private static bool loaded = false;
		private void LoadDatabase()
		{
			if (loaded) return;
			loaded = true;

			string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			string fullPath = Path.Combine(baseDirectory, "Assets", "test_users.xml");

			XmlDocument doc = new XmlDocument();
			doc.Load(fullPath);

			foreach (XmlNode userNode in doc.DocumentElement.ChildNodes)
			{
				User user = new User()
				{
					Username = userNode["Username"].InnerText,
					Password = userNode["Password"].InnerText,
					Name = userNode["Name"].InnerText,
					Surname = userNode["Surname"].InnerText,
					Email = userNode["Email"].InnerText,
					Birthday = userNode["Birthday"].InnerText,
					Gender = (Gender)Enum.Parse(typeof(Gender), userNode["Gender"].InnerText.ToUpper()),
					UserType = (UserType)Enum.Parse(typeof(UserType), userNode["UserType"].InnerText.ToUpper()),

				};

				XmlNodeList reservationNodes = userNode.SelectNodes("ReservationList/Reservation");
				foreach (XmlNode reservationNode in reservationNodes)
				{
					Reservation reservation = new Reservation();
					reservation.User = new User() { Username = reservationNode["User"].InnerText };
					XmlNode flightNode = reservationNode["Flight"];
					Flight flight = new Flight()
					{
						Aviokompanija = new Airline(flightNode["Aviokompanija"].InnerText),
						StartDestination = flightNode["StartDestination"].InnerText,
						EndDestination = flightNode["EndDestination"].InnerText,
						DepartureDateTime = flightNode["DepartureDateTime"].InnerText,
						ArrivalDateTime = flightNode["ArrivalDateTime"].InnerText,
						NumberOf_FreeSeats = int.Parse(flightNode["NumberOf_FreeSeats"].InnerText),
						NumberOf_TakenSeats = int.Parse(flightNode["NumberOf_TakenSeats"].InnerText),
						Price = double.Parse(flightNode["Price"].InnerText.Replace(',', '.')),
						Status = (FlightStatus)Enum.Parse(typeof(FlightStatus), flightNode["Status"].InnerText.ToUpper())
					};

					reservation.Flight = flight; 
					reservation.NumberOfPassangers = int.Parse(reservationNode["NumberOfPassangers"].InnerText);
					reservation.Price = double.Parse(reservationNode["Price"].InnerText.Replace(',', '.'));
					user.ReservationList.Add(reservation);
				}
				users.Add(user);
			}
		}

		[HttpGet]
        public IHttpActionResult Get()
        {
			if (!loaded)
			{
				LoadDatabase();
			}
			return Ok(users);
		}

		[HttpPost]
		public IHttpActionResult Post([FromBody]User user)
        {
			User oldUser = users.Find(x => x.Username == user.Username);

			if(oldUser != null)		//Replace already existing user info
			{
				users.Remove(oldUser);
			}
			users.Add(user);
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

			User currentUser = users.Find(x => x.Username == username);

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

			User currentUser = users.Find(x => x.Username == username);

			var filteredReservations = currentUser.ReservationList
				.Where(r => r.Status == reservationStatus)
				.ToList();
			return Ok(filteredReservations);
		}
	}
}
