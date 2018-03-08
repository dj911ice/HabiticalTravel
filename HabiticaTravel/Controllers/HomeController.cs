using HabiticaTravel.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HabiticaTravel.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                //send them to the AuthenticatedIndex page instead of the index page
                var currentUser = System.Web.HttpContext.Current
                    .GetOwinContext()
                    .GetUserManager<ApplicationUserManager>()
                    .FindById(System.Web.HttpContext
                    .Current.User.Identity
                    .GetUserId());

                var ORM = new habiticatravelEntities();

                ViewBag.Tasks = ORM.CustomTasks.Where(t => t.UserId == currentUser.Id).ToList();

                return View();
            }
            return View("../Home/NotAuthorized", "../Shared/_NotAuthorized");
        }
    }
}