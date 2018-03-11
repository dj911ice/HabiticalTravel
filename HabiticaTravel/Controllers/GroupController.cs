using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HabiticaTravel.Models;

namespace HabiticaTravel.Controllers
{
    public class GroupController : Controller
    {
        // GET: Group
        public ActionResult DisplayGroup() // Essentially our index View
        {
            var HabiticaORM = new habiticatravelEntities();

            ViewBag.GroupUsers = HabiticaORM.UserGroupScores.ToList();


            return View();
        }
    }
}