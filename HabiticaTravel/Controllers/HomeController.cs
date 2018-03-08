using HabiticaTravel.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Linq;

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

        public ActionResult GetUserStats()
        {
            string CurrentUser = "", CurrentApiToken = "";
            ApplicationUser MyUser;

            habiticatravelEntities MyHabitica = new habiticatravelEntities();
            MyUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var MyHabiticaUser = MyHabitica.HabiticaUsers.SqlQuery(sql: "select top 1 * from dbo.HabiticaUsers where UserId = @MyUser", parameters: new SqlParameter("@MyUser", MyUser)).ToList();

            CurrentUser = MyHabiticaUser[0].HabiticaUserId.ToString();
            CurrentApiToken = MyHabiticaUser[0].ApiToken.ToString();
            
            HttpWebRequest MyRequest = WebRequest.CreateHttp("https://habitica.com/api/v3/user");

            MyRequest.UserAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36";

            MyRequest.Headers["x-api-user"] = $"{CurrentUser}";
            MyRequest.Headers["x-api-key"] = $"{CurrentApiToken}";
            
            HttpWebResponse MyResponse = (HttpWebResponse)MyRequest.GetResponse();

            if (MyResponse.StatusCode == HttpStatusCode.OK)
            {
                StreamReader MyReader = new StreamReader(MyResponse.GetResponseStream());

                string Output = MyReader.ReadToEnd(); //reads the entire response back

                //now parse the json/xml data to html for the view

                JObject JParser = JObject.Parse(Output);


                return RedirectToAction("DisplayStats",JParser);
            }
            else
            {
                //can email/save to file the error code for development
                return View("../Shared/Error");
            }
        }

        public ActionResult DisplayStats(JObject JParser)
        {
            if (JParser == null)
            {
                throw new ArgumentNullException(nameof(JParser));
            }

            habiticatravelEntities MyHabitica = new habiticatravelEntities();
            ApplicationUser MyUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());

            UserStat NewStat = new UserStat
            {
                UserId = MyUser.Id,
                UserStatsPer = (int)JParser["stats"]["per"],
                UserStats_int = (int)JParser["stats"]["int"],
                UserStatsCon = (int)JParser["stats"]["con"],
                UserStatsStr = (int)JParser["stats"]["str"],
                UserStatsPoints = (int)JParser["stats"]["points"],
                UserStats_class = JParser["stats"]["class"].ToString(),
                UserStatsLvl = (int)JParser["stats"]["lvl"],
                UserStatsGp = (int)JParser["stats"]["gp"],
                UserStatsExp = (int)JParser["stats"]["exp"],
                UserStatsMp = (int)JParser["stats"]["mp"],
                UserStatsHp = (int)JParser["stats"]["hp"],
                UserStatsToNextLevel = (int)JParser["stats"]["toNextLevel"],
                UserStatsMaxHealth = (int)JParser["stats"]["maxHealth"],
                UserStatsMaxMP = (int)JParser["stats"]["maxMP"],
                TrainingCon = (int)JParser["stats"]["training"]["con"],
                TrainingStr = (int)JParser["stats"]["training"]["str"],
                TrainingPer = (int)JParser["stats"]["training"]["per"],
                Training_int = (int)JParser["stats"]["training"]["int"],
                BuffsSeafoam = (bool)JParser["stats"]["buffs"]["seafoam"],
                BuffsShinySeed = (bool)JParser["stats"]["buffs"]["shinySeed"],
                BuffsSpookySparkles = (bool)JParser["stats"]["buffs"]["spookySparkles"],
                BuffsSnowball = (bool)JParser["stats"]["buffs"]["snowball"],
                BuffsStreaks = (bool)JParser["stats"]["buffs"]["streaks"],
                BuffsStealth = (int)JParser["stats"]["buffs"]["stealth"],
                BuffsCon = (int)JParser["stats"]["buffs"]["con"],
                BuffsPer = (int)JParser["stats"]["buffs"]["per"],
                Buffs_int = (int)JParser["stats"]["buffs"]["int"],
                BuffsStr = (int)JParser["stats"]["buffs"]["str"],
                ProfileName = JParser["profile"]["name"].ToString(),
                ProfilePhoto = JParser["profile"][""].ToString(),
                ProfileBlurb = JParser["profile"]["blurb"].ToString()
            };

            return View("ShowProfile");
        }
    }
}