using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HabiticaTravel.Controllers.Habitica
{
    public partial class HabiticaController : Controller
    {
        // GET: HabiticaAccount

        public ActionResult HabiticaLogin()
        {
            return View();
        }
        public ActionResult HabiticaAccountCredentials(string Email, string Password)
        {
            return View();
        }
        public ActionResult HabiticaUserForm()
        {
            return View();
        }
    }
}