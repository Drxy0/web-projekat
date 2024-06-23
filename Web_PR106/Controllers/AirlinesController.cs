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
    public class AirlinesController : ApiController
    {

		[HttpGet]
        public IHttpActionResult Get()
        {
            return Ok(Global.Airlines);
        }

    }
}
