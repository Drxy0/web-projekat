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
        public IHttpActionResult Post([FromBody]Airline airline)
        {
            if (airline != null)
            {
                Global.Airlines.Add(airline);
                return Ok();
            }
            
            return BadRequest();
        }

<<<<<<< HEAD
=======
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
>>>>>>> 5b1f20d (Add data persistance)

		[HttpPost]
		[Route("adminFilterAirlines")]
		public IHttpActionResult AdminFilterUsers([FromBody] JObject searchFilter)
		{

			if (searchFilter == null)
			{
				return BadRequest();
			}
			List<Airline> filteredAirlines = new List<Airline>(Global.Airlines);

			string name = searchFilter["name"]?.ToString(); // Accessing "name" property from JSON
			string address = searchFilter["address"]?.ToString(); // Accessing "address" property from JSON
			string contactInfo = searchFilter["contactInfo"]?.ToString(); // Accessing "contactInfo" property from JSON

			
			//if form is left unfilled then reset
			if (string.IsNullOrEmpty(name) &&
				string.IsNullOrEmpty(address) &&
				string.IsNullOrEmpty(contactInfo))
			{
				return Ok(Global.Airlines);
			}

			if (!string.IsNullOrEmpty(name))
			{
				foreach (Airline airline in Global.Airlines)
				{
					if (airline.Name != name)
					{
						filteredAirlines.Remove(airline);
					}
				}
			}

			if (!string.IsNullOrEmpty(address))
			{
				foreach (Airline airline in Global.Airlines)
				{
					if (airline.Address != address)
					{
						filteredAirlines.Remove(airline);
					}
				}
			}
			if (!string.IsNullOrEmpty(contactInfo))
			{
				foreach (Airline airline in Global.Airlines)
				{
					if (airline.ContactInfo != contactInfo)
					{
						filteredAirlines.Remove(airline);
					}
				}
			}

			return Ok(filteredAirlines);
		}

		[HttpDelete]
		public IHttpActionResult Delete(int id)
		{
			var airline = Global.Airlines.FirstOrDefault(u => u.Id == id);
			if (airline == null)
			{
				return NotFound();
			}

<<<<<<< HEAD
			airline.IsDeleted = true;
			return Ok();
=======
			if (!airline.ProvidedFlights.Any(f => f.Status == FlightStatus.AKTIVAN))
			{
				airline.IsDeleted = true;
				foreach(Flight flight in airline.ProvidedFlights)
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
				return Content(HttpStatusCode.NotFound,"Airline with that id not found.");
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
>>>>>>> 5b1f20d (Add data persistance)
		}
	}
}
