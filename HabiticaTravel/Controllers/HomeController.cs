using HabiticaTravel.Models;
using HabiticaTravel.Utility;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HabiticaTravel.Controllers
{
    public class HomeController : Controller
    {
        private UserStat userStats;

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                //send them to the AuthenticatedIndex page instead of the index page

                return RedirectToAction("DisplayStats");
            }
            return View("../Home/NotAuthorized", "../Shared/_NotAuthorized");
        }


        public async Task<ActionResult> DisplayStats()
        {
            var userId = User.Identity.GetUserId();
            var data = await HabiticaUtil.GetUserStats(userId);
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
                userStats = new UserStat
                {
                    UserId = userId,
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
                    ProfileName = data["profile"]["name"].ToString()
                };
                if (data["profile"].Contains("imageUrl"))
                {
                    userStats.ProfilePhoto = data["profile"]["imageUrl"].ToString();
                }
                if (data["profile"].Contains("blurb"))
                {
                    userStats.ProfileBlurb = data["profile"]["blurb"].ToString();
                }

                habiticatravelEntities orm = new habiticatravelEntities();
                List<CustomTask> Tasks = orm.CustomTasks.Where(t => t.UserId == userId).ToList();

                var taskDates = Tasks.Select(t => t.ReminderTime).ToList();
                var reminderEndTime = new List<DateTime>();

                ViewBag.Tasks = Tasks;
                return View("index", userStats);
            }
        }
    }
}