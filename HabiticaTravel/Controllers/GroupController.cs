using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HabiticaTravel.Controllers
{
    public class GroupController : Controller
    {
        // GET: Group
        public ActionResult DisplayGroup() // Essentially our index View
        {
            return View();
        }
    }
}