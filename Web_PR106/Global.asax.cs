using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using System.Xml;
using Web_PR106.Models;
using System.IO;
using System.Diagnostics;
using System.Timers;
using System.Globalization;
using System.Xml.Serialization;

namespace Web_PR106
{
	public class Global : HttpApplication
	{
		public static List<Airline> Airlines = new List<Airline>();
		public static List<Flight> Flights = new List<Flight>();
		public static List<Flight> ShownFlights = new List<Flight>();
		public static List<User> Users = new List<User>();
		public static List<Reservation> Reservations = new List<Reservation>();
		public static int ReviewIdCounter = 20;
		private static Timer timer;

		void Application_Start(object sender, EventArgs e)
		{
			// Code that runs on application startup
			AreaRegistration.RegisterAllAreas();
			GlobalConfiguration.Configure(WebApiConfig.Register);
			RouteConfig.RegisterRoutes(RouteTable.Routes);

			LoadDatabaseAirlines();
			LoadFlights();
			LoadUsers();

			timer = new Timer(30000);
			timer.Elapsed += TimerElapsed;
			timer.AutoReset = true;
			timer.Start();
		}

		private void Application_PostAuthorizeRequest()
		{
			HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
		}

		public static int SetFlightId()
		{
			int id = 1;
			bool isUnique = false;

			while (!isUnique)
			{
				isUnique = true;
				foreach (Airline airline in Airlines)
				{
					foreach (Flight flight in airline.ProvidedFlights)
					{
						if (flight.Id == id)
						{
							// ID is not unique, generate a new one
							isUnique = false;
							break;
						}
					}

					if (!isUnique)
					{
						break;
					}
				}
				// If ID is unique, return it; otherwise, generate a new one
				if (!isUnique)
				{
					id++;
				}
			}
			return id;
		}

		public static int SetReservationId()
		{
			int id = 1;
			bool isUnique = false;

			while (!isUnique)
			{
				isUnique = true;
				foreach (User user in Users)
				{
					foreach (Reservation res in user.ReservationList)
					{
						if (res.Id == id)
						{
							isUnique = false;
							break;
						}
					}
					if (!isUnique)
					{
						break;
					}
				}
				if (!isUnique)
				{
					id++;
				}
			}
			return id;
		}

		public static void SaveAirlineData()
		{
			XmlSerializer serializer = new XmlSerializer(typeof(List<Airline>), new XmlRootAttribute("Airlines"));
			string relativePath = "~/Assets/test_aviokompanije.xml";
			string filePath = HttpContext.Current.Server.MapPath(relativePath);
			using (TextWriter writer = new StreamWriter(filePath))
			{
				serializer.Serialize(writer, Global.Airlines);
			}
			LoadDatabaseAirlines();
			LoadFlights();
		}
		public static void SaveUserData()
		{
			XmlSerializer serializer = new XmlSerializer(typeof(List<User>), new XmlRootAttribute("Users"));
			string relativePath = "~/Assets/test_users.xml";
			string filePath = HttpContext.Current.Server.MapPath(relativePath);
			using (TextWriter writer = new StreamWriter(filePath))
			{
				serializer.Serialize(writer, Global.Users);
			}
			LoadUsers();
		}
		private static void TimerElapsed(object sender, ElapsedEventArgs e)
		{
			UpdateFlightStatus();
		}

		private static void UpdateFlightStatus()
		{
			DateTime now = DateTime.Now;
			DateTime arrivalDateTime;
			foreach (Airline airline in Airlines)
			{
				foreach (Flight flight in airline.ProvidedFlights)
				{
					arrivalDateTime = DateTime.ParseExact(flight.ArrivalDateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
					if (arrivalDateTime < now && flight.Status != FlightStatus.OTKAZAN)
					{
						flight.Status = FlightStatus.OTKAZAN;
					}
				}
			}
			//SaveAirlineData();
		}
		public static void LoadUsers()
		{
			Users.Clear();

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
					reservation.User = reservationNode["User"].InnerText;
					XmlNode flightNode = reservationNode["Flight"];
					Flight flight = new Flight()
					{
						Id = int.Parse(flightNode["Id"].InnerText),
						Airline = new Airline()
						{
							Id = int.Parse(flightNode["Airline"]["Id"].InnerText),
							Name = flightNode["Airline"]["Name"].InnerText
						},
						StartDestination = flightNode["StartDestination"].InnerText,
						EndDestination = flightNode["EndDestination"].InnerText,
						DepartureDateTime = flightNode["DepartureDateTime"].InnerText,
						ArrivalDateTime = flightNode["ArrivalDateTime"].InnerText,
						NumberOf_FreeSeats = int.Parse(flightNode["NumberOf_FreeSeats"].InnerText),
						NumberOf_TakenSeats = int.Parse(flightNode["NumberOf_TakenSeats"].InnerText),
						Price = double.Parse(flightNode["Price"].InnerText.Replace('.', ',')),
						Status = (FlightStatus)Enum.Parse(typeof(FlightStatus), flightNode["Status"].InnerText.ToUpper())
					};
					reservation.Id = int.Parse(reservationNode["Id"].InnerText);
					reservation.Flight = flight;
					reservation.NumberOfPassengers = int.Parse(reservationNode["NumberOfPassengers"].InnerText);
					reservation.Price = double.Parse(reservationNode["Price"].InnerText);
					reservation.Status = (ReservationStatus)Enum.Parse(typeof(ReservationStatus), reservationNode["Status"].InnerText.ToUpper());
					user.ReservationList.Add(reservation);
					Reservations.Add(reservation);
				}
				Users.Add(user);
			}
		}

		public static void LoadFlights()
		{
			Flights.Clear();
			foreach (Airline airline in Airlines)
			{
				foreach (Flight flight in airline.ProvidedFlights)
				{
					if (!Flights.Contains(flight))
					{
						Flights.Add(flight);
					}
				}
			}
			ShownFlights = new List<Flight>(Flights);

		}
		public static void LoadDatabaseAirlines()
		{
			Airlines.Clear();

			string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			string fullPath = baseDirectory + "\\Assets\\test_aviokompanije.xml";

			XmlDocument doc = new XmlDocument();
			doc.Load(fullPath);

			foreach (XmlNode node in doc.DocumentElement.ChildNodes)
			{
				Airline airline = new Airline()
				{
					Id = int.Parse(node["Id"].InnerText),
					Name = node["Name"].InnerText,
					Address = node["Address"].InnerText,
					ContactInfo = node["ContactInfo"].InnerText,
					IsDeleted = bool.Parse(node["IsDeleted"].InnerText),
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
							Airline = new Airline()
							{
								Id = int.Parse(flightNode["Airline"]["Id"].InnerText),
								Name = flightNode["Airline"]["Name"].InnerText
							},
							Id = int.Parse(flightNode["Id"].InnerText),
							StartDestination = flightNode["StartDestination"].InnerText,
							EndDestination = flightNode["EndDestination"].InnerText,
							DepartureDateTime = flightNode["DepartureDateTime"].InnerText,
							ArrivalDateTime = flightNode["ArrivalDateTime"].InnerText,
							NumberOf_FreeSeats = int.Parse(flightNode["NumberOf_FreeSeats"].InnerText),
							NumberOf_TakenSeats = int.Parse(flightNode["NumberOf_TakenSeats"].InnerText),
							Price = double.Parse(flightNode["Price"].InnerText.Replace('.', ',')),
							Status = (FlightStatus)Enum.Parse(typeof(FlightStatus), flightNode["Status"].InnerText),
							IsDeleted = bool.Parse(flightNode["IsDeleted"].InnerText),
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
							Id = int.Parse(reviewNode["Id"].InnerText),
							Reviewer = reviewNode["Reviewer"].InnerText,
							Airline = reviewNode["Airline"].InnerText,
							Title = reviewNode["Title"].InnerText,
							Description = reviewNode["Description"].InnerText,
							Picture = reviewNode["Picture"].InnerText,
							Status = (ReviewStatus)Enum.Parse(typeof(ReviewStatus), reviewNode["Status"].InnerText),
							IsDeleted = bool.Parse(reviewNode["IsDeleted"].InnerText),
						};
						airline.Reviews.Add(review);
					}
				}
				Airlines.Add(airline);
			}
		}
	}
}