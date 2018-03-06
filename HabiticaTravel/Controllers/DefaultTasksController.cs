using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HabiticaTravel.Models;

namespace HabiticaTravel.Controllers
{
    public class DefaultTasksController : Controller
    {
        // GET: DefaultTasks
        public ActionResult ShowDefaultTasks()
        {
            habiticatravelEntities MyHabitica = new habiticatravelEntities();
            

            return View();
        }
    }
}