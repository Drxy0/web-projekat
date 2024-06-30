using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Http;
using System.Xml;
using System.Xml.Serialization;
using Web_PR106.Models;

namespace Web_PR106.Controllers
{
	[RoutePrefix("api/airlines")]
	public class AirlinesController : ApiController
	{
		[HttpGet]
		public IHttpActionResult Get()
		{
			return Ok(Global.Airlines);
		}

		[HttpPost]
		public IHttpActionResult Post([FromBody] Airline airline)
		{
			if (airline == null)
			{
				return BadRequest();
			}
			bool airlineExists = Global.Airlines.Any(u => u.Id == airline.Id);
			if (!airlineExists)
			{
				Global.Airlines.Add(airline);
				Global.SaveAirlineData();
				return Ok();
			}
			return BadRequest("Airline with that id already exists.");
		}

		[HttpPost]
		[Route("adminFilterAirlines")]
		public IHttpActionResult AdminFilterUsers([FromBody] JObject searchFilter)
		{

			if (searchFilter == null)
			{
				return BadRequest();
			}
			List<Airline> filteredAirlines = new List<Airline>(Global.Airlines);

			string name = searchFilter["name"]?.ToString();
			string address = searchFilter["address"]?.ToString();
			string email = searchFilter["email"]?.ToString();
			string phone = searchFilter["phone"]?.ToString();


			//if form is left unfilled then reset
			if (string.IsNullOrEmpty(name) &&
				string.IsNullOrEmpty(address) &&
				string.IsNullOrEmpty(email) &&
				string.IsNullOrEmpty(phone))
			{
				return Ok(Global.Airlines);
			}

			if (!string.IsNullOrEmpty(name))
			{
				foreach (Airline airline in Global.Airlines)
				{
					if (!airline.Name.Contains(name))
					{
						filteredAirlines.Remove(airline);
					}
				}
			}

			if (!string.IsNullOrEmpty(address))
			{
				foreach (Airline airline in Global.Airlines)
				{
					if (!airline.Address.Contains(address))
					{
						filteredAirlines.Remove(airline);
					}
				}
			}

			if (!string.IsNullOrEmpty(email))
			{
				foreach (Airline airline in Global.Airlines)
				{
					int emailStartIndex = airline.ContactInfo.IndexOf("email: ") + "email: ".Length;
					int emailEndIndex = airline.ContactInfo.IndexOf("/phone: ");
					string airlineEmail = airline.ContactInfo.Substring(emailStartIndex, emailEndIndex - emailStartIndex).Trim();

					if (!airlineEmail.Contains(email))
					{
						filteredAirlines.Remove(airline);
					}
				}
			}

			if (!string.IsNullOrEmpty(phone))
			{
				foreach (Airline airline in Global.Airlines)
				{
					int phoneStartIndex = airline.ContactInfo.IndexOf("/phone: ") + "/phone: ".Length;
					string airlinePhone = airline.ContactInfo.Substring(phoneStartIndex).Trim();
					if (!airlinePhone.Contains(phone))
					{
						filteredAirlines.Remove(airline);
					}
				}
			}

			return Ok(filteredAirlines);
		}

		[HttpDelete]
		[Route("delete/{id}")]
		public IHttpActionResult Delete(int id)
		{
			var airline = Global.Airlines.FirstOrDefault(u => u.Id == id);
			if (airline == null)
			{
				return NotFound();
			}

			if (!airline.ProvidedFlights.Any(f => f.Status == FlightStatus.AKTIVAN))
			{
				airline.IsDeleted = true;
				foreach (Flight flight in airline.ProvidedFlights)
				{
					flight.IsDeleted = true;
					flight.Airline.IsDeleted = true;
				}
				foreach (Review review in airline.Reviews)
				{
					review.IsDeleted = true;
				}

				Global.SaveAirlineData();

				return Ok();
			}
			return BadRequest("Cannot delete airline with active flights.");
		}

		[HttpPost]
		[Route("edit")]
		public IHttpActionResult Edit([FromBody] Airline airline)
		{
			if (airline == null)
			{
				return BadRequest();
			}
			bool airlineExists = Global.Airlines.Any(u => u.Id == airline.Id);
			if (!airlineExists)
			{
				return Content(HttpStatusCode.NotFound, "Airline with that id not found.");
			}
			else
			{
				Airline airlineFromDB = Global.Airlines.FirstOrDefault(u => u.Id == airline.Id);
				airlineFromDB.Name = airline.Name;
				airlineFromDB.Address = airline.Address;
				airlineFromDB.ContactInfo = airline.ContactInfo;

				Global.SaveAirlineData();
				return Ok();
			}
		}
	}
}
