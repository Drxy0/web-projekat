using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Web_PR106.Models;

namespace Web_PR106.Controllers
{
    public class UsersController : ApiController
    {
        private static List<User> users = new List<User>();

        [HttpGet]
        public IHttpActionResult Get()
        {
			try
			{
				Trace.WriteLine(users.Count);
				return Ok(users);
			}
			catch (Exception ex)
			{
				Trace.WriteLine($"Error in Get method: {ex.Message}");
				return InternalServerError(ex);
			}
		}


        [HttpPost]
        public IHttpActionResult Post([FromBody]User user)
        {
            users.Add(user);
            return Ok();
        }
    }
}
