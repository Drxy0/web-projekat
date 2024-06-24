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
    public class ReviewController : ApiController
    {
		[HttpPost]
		public IHttpActionResult Post([FromBody]string review)
		{
			//Airline airline = Global.Airlines.Find(x => x.Name == review.Airline);
			//airline.Reviews.Add(review);
			Trace.WriteLine("lmao");
			Trace.WriteLine(review);
			return Ok();
		}
	}
}
