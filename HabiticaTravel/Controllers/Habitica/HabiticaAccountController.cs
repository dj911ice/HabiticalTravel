using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HabiticaTravel.Models;
using HabiticaTravel.Utility;
using Newtonsoft.Json.Linq;


namespace HabiticaTravel.Controllers.Habitica
{
    public partial class HabiticaController : Controller
    {
        // GET: HabiticaAccount

        public ActionResult HabiticaLogin()
        {
            return View();
        }
        public ActionResult HabiticaAccountCredentials(string UserName, string Password)
        {
            return View();
        }

        public async Task<ActionResult> RegisterNewUser( )
        {
            var model = (RegisterViewModel)TempData["model"];

            var user = (ApplicationUser)TempData["user"];
            
            var output = await HabiticaPost.RegisterNewUser(user, model);

            var strOutput = output.ToString();

            var JSON = JObject.Parse(strOutput);

            var ORM = new habiticatravelEntities();

            var IdenUser = new ApplicationDbContext();

            ApplicationUser DBUser = IdenUser.Users.Find(user.UserName);

            

            var HabUser = new HabiticaUser();

            HabUser.ApiToken = (string)JSON["data"]["apiToken"];

            HabUser.Uuid = (string)JSON["data"]["id"];

            HabUser.UserId = DBUser.UserName;

            ORM.HabiticaUsers.Add(HabUser);

            ORM.SaveChanges();

            return RedirectToAction("Index", "Home");

        }

        


    }
}