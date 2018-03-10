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

                return RedirectToAction("DisplayStats");
            }
            return View("../Home/NotAuthorized", "../Shared/_NotAuthorized");
        }


        public async Task<ActionResult> DisplayStats()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current
                   .GetOwinContext()
                   .GetUserManager<ApplicationUserManager>()
                   .FindById(System.Web.HttpContext
                   .Current.User.Identity
                   .GetUserId());

            var data = await HabiticaUtil.GetUserStats(currentUser);
            data = (JObject)data["data"];

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (!System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }
            else
            {

                ApplicationUser MyUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());

                var NewStat = new UserStat();
                NewStat.UserId = MyUser.Id;
                NewStat.UserStatsPer = (int)data["stats"]["per"];
                NewStat.UserStats_int = (int)data["stats"]["int"];
                NewStat.UserStatsCon = (int)data["stats"]["con"];
                NewStat.UserStatsStr = (int)data["stats"]["str"];
                NewStat.UserStatsPoints = (int)data["stats"]["points"];
                NewStat.UserStats_class = data["stats"]["class"].ToString();
                NewStat.UserStatsLvl = (int)data["stats"]["lvl"];
                NewStat.UserStatsGp = (int)data["stats"]["gp"];
                NewStat.UserStatsExp = (int)data["stats"]["exp"];
                NewStat.UserStatsMp = (int)data["stats"]["mp"];
                NewStat.UserStatsHp = (int)data["stats"]["hp"];
                NewStat.UserStatsToNextLevel = (int)data["stats"]["toNextLevel"];
                NewStat.UserStatsMaxHealth = (int)data["stats"]["maxHealth"];
                NewStat.UserStatsMaxMP = (int)data["stats"]["maxMP"];
                NewStat.TrainingCon = (int)data["stats"]["training"]["con"];
                NewStat.TrainingStr = (int)data["stats"]["training"]["str"];
                NewStat.TrainingPer = (int)data["stats"]["training"]["per"];
                NewStat.Training_int = (int)data["stats"]["training"]["int"];
                NewStat.BuffsSeafoam = (bool)data["stats"]["buffs"]["seafoam"];
                NewStat.BuffsShinySeed = (bool)data["stats"]["buffs"]["shinySeed"];
                NewStat.BuffsSpookySparkles = (bool)data["stats"]["buffs"]["spookySparkles"];
                NewStat.BuffsSnowball = (bool)data["stats"]["buffs"]["snowball"];
                NewStat.BuffsStreaks = (bool)data["stats"]["buffs"]["streaks"];
                NewStat.BuffsStealth = (int)data["stats"]["buffs"]["stealth"];
                NewStat.BuffsCon = (int)data["stats"]["buffs"]["con"];
                NewStat.BuffsPer = (int)data["stats"]["buffs"]["per"];
                NewStat.Buffs_int = (int)data["stats"]["buffs"]["int"];
                NewStat.BuffsStr = (int)data["stats"]["buffs"]["str"];
                NewStat.ProfileName = data["profile"]["name"].ToString();

                if (data["profile"].Contains("imageUrl"))
                {
                    NewStat.ProfilePhoto = data["profile"]["imageUrl"].ToString();
                }
                if (data["profile"].Contains("blurb"))
                {
                    NewStat.ProfileBlurb = data["profile"]["blurb"].ToString();
                }

                habiticatravelEntities orm = new habiticatravelEntities();
                List<CustomTask> Tasks = orm.CustomTasks.Where(t => t.UserId == currentUser.Id).ToList();



                var taskDates = Tasks.Select(t => t.ReminderTime).ToList();
                var reminderEndTime = new List<DateTime>();



                foreach (var date in taskDates)
                {
                    if (date == null)
                    {
                        continue;
                    }
                    reminderEndTime.Add((DateTime)date);
                }

                ViewBag.EndDates = reminderEndTime;
                ViewBag.Tasks = Tasks;
                return View("index", NewStat);
            }
        }
    }
}