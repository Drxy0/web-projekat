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
			// Access form data from the request
			var httpRequest = HttpContext.Current.Request;
			
			// Retrieve and parse JSON data from FormData
			string jsonData = httpRequest.Form["data"];
			var review = JsonConvert.DeserializeObject<Review>(jsonData);

			if (httpRequest.Files.Count > 0)
			{
				var postedFile = httpRequest.Files[0];
				var filePath = HttpContext.Current.Server.MapPath($"~/UploadedFiles/{postedFile.FileName}");
				postedFile.SaveAs(filePath);

				review.Picture = postedFile.FileName;
				//review.UploadedFileUrl = $"/UploadedFiles/{postedFile.FileName}";
			}
			else
			{
				review.Picture = null;
				//review.UploadedFileUrl = null;
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
		private string SaveImage(HttpPostedFileBase imageFile)
		{
			// Example implementation: Save the image file to a location
			// For demo purposes, you might save to a directory or a cloud storage

			// Example: Save to local directory
			string fileName = Path.GetFileName(imageFile.FileName);
			string filePath = HttpContext.Current.Server.MapPath("~/App_Data/uploads/" + fileName);
			imageFile.SaveAs(filePath);

			// Example: Return the URL to access the uploaded image
			return "/App_Data/uploads/" + fileName;
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
			return Ok(new { message = $"Review with ID {Id} has been rejected." });
		}
	}
}
