using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HabiticaTravel.Models;

namespace HabiticaTravel.Controllers
{
    public class TasksController : Controller
    {
        // GET: DefaultTasks
        public ActionResult DefaultTasks()
        {
            habiticatravelEntities MyHabitica = new habiticatravelEntities();

            ViewBag.MyTasks = MyHabitica.DefaultTasks.ToList();
            ViewBag.MyTaskItems = MyHabitica.DefaultTaskItems.ToList();
            

            return View();
        }
    }
}