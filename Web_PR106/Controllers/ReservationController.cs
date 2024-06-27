using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Web_PR106.Controllers
{
    public class ReservationController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok(Global.Reservations);
        }
    }
}
