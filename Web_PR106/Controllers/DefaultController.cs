using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using System.IO;
using System.Text.Json;
using Web_PR106.Models;

namespace Web_PR106.Controllers
{
    public class DefaultController : ApiController
    {
		[HttpGet, Route("")]
		public RedirectResult Index()
		{
			string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			string fullPath = baseDirectory + "\\Assets\\test_data.json";
			string jsonString = File.ReadAllText(fullPath);
			DataModel data = JsonSerializer.Deserialize<DataModel>(jsonString);
			Aviokompanija avioKompanija = data.Aviokompanija;
			Korisnik user = data.Korisnik;
			Let flight = data.Let;
			Recenzija review = data.Recenzija;
			Rezervacija rezervacija = data.Rezervacija;


			var requestUri = Request.RequestUri;
			return Redirect(requestUri.AbsoluteUri + "Index.html");
		}
	}
}
