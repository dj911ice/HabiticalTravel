using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using HabiticaTravel.Models;
using HabiticaTravel.Utility;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json.Linq;


namespace HabiticaTravel.Controllers.Habitica
{
    public partial class HabiticaController : Controller
    {
        private UserManager<ApplicationUser> userManager;
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
            // stored the model and user into temp data, found it much easier to store this data
            // in TempData as opposed to passing it as a Dictionary and use parameters
            var model = (RegisterViewModel)TempData["model"];
            var user = (ApplicationUser)TempData["user"];

            // stored the outpout, which is a string, into and then did the classic JObject parsing,
            // so we can navigate the key,values dictionary style. 
            string output = await HabiticaPost.RegisterNewUser(user, model);
            var JSON = JObject.Parse(output);

            // these are both of our ORM copies, so the the first one is our Habitica User,
            // second ORM is basicaly the an object that wraps around our ApplicationUser, the reason
            // we have to do it this way is because UserManager allows us to preform CRUD operation
            // to our Identity database. 
            var HabiticaORM = new habiticatravelEntities();
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            // simply finding the user in our Identity database by whatever the user put into
            // the User Name form field. 
            var idenUser = userManager.FindByName(user.UserName);

            // creating a new HabiticaUser object which will store the required Non-NULL values, we did
            // not include the data for HabiticaUserID because that will auto increment in the database.
            var HabiticaUser = new HabiticaUser
            {
                ApiToken = (string)JSON["data"]["apiToken"],
                Uuid = (string)JSON["data"]["id"],
                UserId = idenUser.Id
            };

            // finishing off our CRUD operation, we are adding the new HabiticaUser into our database
            // and saving our changes so it wil reflect in the database.  then we are simply returning 
            // the HomePage view for now. 
            HabiticaORM.HabiticaUsers.Add(HabiticaUser);
            HabiticaORM.SaveChanges();
            return RedirectToAction("Index", "Home");

        }

        


    }
}