using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.SessionState;
using System.Diagnostics;

namespace Web_PR106.Controllers
{
	[RoutePrefix("api/session")]
	public class SessionController : ApiController
	{

		[HttpPost]
		[Route("logIn")]
		public IHttpActionResult LogIn([FromBody] JObject userInfo)
		{

			HttpSessionState session = HttpContext.Current.Session;
			session["token"] = userInfo["username"];
			int role = Convert.ToInt32(userInfo["userType"]);

			if (role == 0)
			{
				session["role"] = "PUTNIK";
			}
			else
			{
				session["role"] = "ADMINISTRATOR";
			}

			return Ok();
		}

		[HttpPost]
		[Route("logOut")]
		public IHttpActionResult LogOut()
		{
			HttpSessionState session = HttpContext.Current.Session;
			session.Clear();
			session.Abandon();

			return Ok();
		}

		[HttpGet]
		[Route("isLoggedIn")]
		public IHttpActionResult IsLoggedIn()
		{
			HttpSessionState session = HttpContext.Current.Session;

			if (session["token"] != null && session["role"] != null)
			{
				var response = new
				{
					token = session["token"],
					role = session["role"]
				};
				return Ok(response);
			}

			return Ok(false);
		}

	}
}
