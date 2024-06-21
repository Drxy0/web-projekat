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
					reservation.Flight = new Flight() { Aviokompanija = new Airline (reservationNode["Flight"].InnerText) };
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
    }
}
