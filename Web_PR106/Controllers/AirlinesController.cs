using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml;
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

			airline.IsDeleted = true;
			return Ok(); // Return 200 OK if deletion is successful
		}
	}
}
