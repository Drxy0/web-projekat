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

namespace Web_PR106
{
    public class Global : HttpApplication
    {
		public static List<Airline> Airlines = new List<Airline>();
		public static List<Flight> Flights = new List<Flight>();
		public static List<Flight> ShownFlights = new List<Flight>();
		public static List<User> Users = new List<User>();

		void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

			LoadDatabaseAirlines();
			LoadFlights();
			LoadUsers();

		}
		private void LoadUsers()
		{
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
						FlightId = int.Parse(flightNode["FlightId"].InnerText),
						Aviokompanija = new Airline(flightNode["Aviokompanija"].InnerText),
						StartDestination = flightNode["StartDestination"].InnerText,
						EndDestination = flightNode["EndDestination"].InnerText,
						DepartureDateTime = flightNode["DepartureDateTime"].InnerText,
						ArrivalDateTime = flightNode["ArrivalDateTime"].InnerText,
						NumberOf_FreeSeats = int.Parse(flightNode["NumberOf_FreeSeats"].InnerText),
						NumberOf_TakenSeats = int.Parse(flightNode["NumberOf_TakenSeats"].InnerText),
						Price = double.Parse(flightNode["Price"].InnerText),
						Status = (FlightStatus)Enum.Parse(typeof(FlightStatus), flightNode["Status"].InnerText.ToUpper())
					};

					reservation.Flight = flight;
					reservation.NumberOfPassengers = int.Parse(reservationNode["NumberOfPassengers"].InnerText);
					reservation.Price = double.Parse(reservationNode["Price"].InnerText.Replace(',', '.'));
					reservation.Status = (ReservationStatus)Enum.Parse(typeof(ReservationStatus), reservationNode["ReservationStatus"].InnerText.ToUpper());
					user.ReservationList.Add(reservation);
				}
				Users.Add(user);
			}
		}

		private void LoadFlights()
		{
			string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			string fullPath = baseDirectory + "\\Assets\\test_letovi.xml";

			XmlDocument doc = new XmlDocument();
			doc.Load(fullPath);

			foreach (XmlNode node in doc.DocumentElement.ChildNodes)
			{
				Flight flight = new Flight();

				flight.FlightId = int.Parse(node["FlightId"].InnerText);
				flight.Aviokompanija = new Airline(node["Aviokompanija"].InnerText);
				flight.StartDestination = node["StartDestination"].InnerText;
				flight.EndDestination = node["EndDestination"].InnerText;
				flight.DepartureDateTime = node["DepartureDateTime"].InnerText;
				flight.ArrivalDateTime = node["ArrivalDateTime"].InnerText;
				flight.NumberOf_FreeSeats = int.Parse(node["NumberOf_FreeSeats"].InnerText);
				flight.NumberOf_TakenSeats = int.Parse(node["NumberOf_TakenSeats"].InnerText);
				flight.Price = double.Parse(node["Price"].InnerText);
				flight.Status = (FlightStatus)Enum.Parse(typeof(FlightStatus), node["Status"].InnerText);

				Flights.Add(flight);
			}
			ShownFlights = new List<Flight>(Flights);
		}
		private void LoadDatabaseAirlines()
		{
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
							//Picture = reviewNode["Picture"].InnerText,
							Status = (ReviewStatus)Enum.Parse(typeof(ReviewStatus), reviewNode["Status"].InnerText)
						};
						airline.Reviews.Add(review);
					}
				}
				Airlines.Add(airline);
			}
		}
	}
}