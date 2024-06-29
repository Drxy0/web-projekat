using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Web_PR106.Models;

namespace Web_PR106.Controllers
{
	[RoutePrefix("api/review")]
    public class ReviewController : ApiController
    {

		[HttpPost]
		[Route("")]
		public IHttpActionResult Post()
		{
			var httpRequest = HttpContext.Current.Request;
			string jsonData = httpRequest.Form["data"];
			var review = JsonConvert.DeserializeObject<Review>(jsonData);

			if (httpRequest.Files.Count > 0)
			{
				var postedFile = httpRequest.Files[0];
				var filePath = HttpContext.Current.Server.MapPath($"~/UploadedImages/{postedFile.FileName}");
				postedFile.SaveAs(filePath);

				review.Picture = $"/UploadedImages/{postedFile.FileName}";
			}
			else
			{
				review.Picture = null;
			}

			foreach(Airline airline in Global.Airlines)
			{
				if (airline.Name == review.Airline)
				{
					airline.Reviews.Add(review);
					break;
				}
			}
			return Ok();
		}

		[HttpPost]
		[Route("approve/{Id}")]
		public IHttpActionResult ApproveReview(int Id)
		{
			foreach(Airline airline in Global.Airlines)
			{
				var review = airline.Reviews.FirstOrDefault(u => u.Id == Id);
				if (review != null)
				{
					review.Status = ReviewStatus.ODOBRENA;
				}
			}

			Global.SaveAirlineData();
			
			return Ok(new { message = $"Review with ID {Id} has been approved." });
		}

		[HttpPost]
		[Route("reject/{Id}")]
		public IHttpActionResult RejectReview(int Id)
		{
			foreach (Airline airline in Global.Airlines)
			{
				var review = airline.Reviews.FirstOrDefault(u => u.Id == Id);
				if (review != null)
				{
					review.Status = ReviewStatus.ODBIJENA;
				}
			}

			Global.SaveAirlineData();
			return Ok(new { message = $"Review with ID {Id} has been rejected." });
		}
	}
}
