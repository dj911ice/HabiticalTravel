using HabiticaTravel.Models;
using HabiticaTravel.Utility;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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


                TempData["userTasks"] = ORM.CustomTasks.Where(t => t.UserId == currentUser.Id).ToList();

                return RedirectToAction("GetUserStats");
            }
            return View("../Home/NotAuthorized", "../Shared/_NotAuthorized");
        }

        public async Task<ActionResult> GetUserStats()
        {
            // string CurrentUser = "", CurrentApiToken = "";
            ApplicationUser MyUser;

            habiticatravelEntities MyHabitica = new habiticatravelEntities();
            MyUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            HabiticaUser MyHabiticaUser = MyHabitica.HabiticaUsers.Where(y => y.UserId == MyUser.Id).FirstOrDefault();

            if (MyUser == null || MyHabiticaUser == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                //CurrentUser = MyHabiticaUser.HabiticaUserId.ToString();
                //CurrentApiToken = MyHabiticaUser.ApiToken.ToString();

                //HttpWebRequest MyRequest = WebRequest.CreateHttp("https://habitica.com/api/v3/user");

                //MyRequest.UserAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36";

                //MyRequest.Headers["x-api-user"] = $"{CurrentUser}";
                //MyRequest.Headers["x-api-key"] = $"{CurrentApiToken}";

                //HttpWebResponse MyResponse = (HttpWebResponse)MyRequest.GetResponse();

                var JSON = (JObject)JObject.FromObject(await HabiticaGet.UserData(MyHabiticaUser));

                if (bool.Parse(JSON["success"].ToString()))
                {
                    //StreamReader MyReader = new StreamReader(MyResponse.GetResponseStream());

                    //string Output = MyReader.ReadToEnd(); //reads the entire response back

                    ////now parse the json/xml data to html for the view

                    //JObject JParser = JObject.Parse(Output);

                    TempData["User"] = JSON["data"];

                    return RedirectToAction("DisplayStats");
                }
                else
                {
                    //can email/save to file the error code for development
                    return View("../Shared/Error");
                }
            }
        }

        public ActionResult DisplayStats()
        {
            var data = (JObject)TempData["User"];
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            habiticatravelEntities MyHabitica = new habiticatravelEntities();

            if (!System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }
            else
            {

                ApplicationUser MyUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
                

                var NewStat = new UserStat
                {
                    UserId = MyUser.Id,
                    UserStatsPer = (int)data["stats"]["per"],
                    UserStats_int = (int)data["stats"]["int"],
                    UserStatsCon = (int)data["stats"]["con"],
                    UserStatsStr = (int)data["stats"]["str"],
                    UserStatsPoints = (int)data["stats"]["points"],
                    UserStats_class = data["stats"]["class"].ToString(),
                    UserStatsLvl = (int)data["stats"]["lvl"],
                    UserStatsGp = (int)data["stats"]["gp"],
                    UserStatsExp = (int)data["stats"]["exp"],
                    UserStatsMp = (int)data["stats"]["mp"],
                    UserStatsHp = (int)data["stats"]["hp"],
                    UserStatsToNextLevel = (int)data["stats"]["toNextLevel"],
                    UserStatsMaxHealth = (int)data["stats"]["maxHealth"],
                    UserStatsMaxMP = (int)data["stats"]["maxMP"],
                    TrainingCon = (int)data["stats"]["training"]["con"],
                    TrainingStr = (int)data["stats"]["training"]["str"],
                    TrainingPer = (int)data["stats"]["training"]["per"],
                    Training_int = (int)data["stats"]["training"]["int"],
                    BuffsSeafoam = (bool)data["stats"]["buffs"]["seafoam"],
                    BuffsShinySeed = (bool)data["stats"]["buffs"]["shinySeed"],
                    BuffsSpookySparkles = (bool)data["stats"]["buffs"]["spookySparkles"],
                    BuffsSnowball = (bool)data["stats"]["buffs"]["snowball"],
                    BuffsStreaks = (bool)data["stats"]["buffs"]["streaks"],
                    BuffsStealth = (int)data["stats"]["buffs"]["stealth"],
                    BuffsCon = (int)data["stats"]["buffs"]["con"],
                    BuffsPer = (int)data["stats"]["buffs"]["per"],
                    Buffs_int = (int)data["stats"]["buffs"]["int"],
                    BuffsStr = (int)data["stats"]["buffs"]["str"],
                    ProfileName = data["profile"]["name"].ToString(),
                };

                if(data["profile"].Contains("imageUrl"))
                {
                    NewStat.ProfilePhoto = data["profile"]["imageUrl"].ToString();
                }
                if(data["profile"].Contains("blurb"))
                {
                    NewStat.ProfileBlurb = data["profile"]["blurb"].ToString();
                }

                List<CustomTask> Tasks = (List<CustomTask>)TempData["userTasks"];

                var reminderEndDates = new List<DateTime?>();

                var taskDates = Tasks.Select(t => t.ReminderTime).ToList();

                foreach (var dates in taskDates)
                {
                    reminderEndDates.Add(dates);
                }

                ViewBag.EndDates = reminderEndDates;
                ViewBag.Tasks = Tasks;
                return View("index", NewStat);
            }
        }
    }
}