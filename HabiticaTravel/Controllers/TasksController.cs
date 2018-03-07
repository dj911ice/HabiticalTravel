using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HabiticaTravel.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;


namespace HabiticaTravel.Controllers
{
    public class TasksController : Controller
    {
        // GET: DefaultTasks
        public ActionResult DefaultTasks()
        {
            habiticatravelEntities MyHabitica = new habiticatravelEntities();

            ViewBag.MyTasks = MyHabitica?.DefaultTasks?.ToList();
            ViewBag.MyTaskItems = MyHabitica?.DefaultTaskItems?.ToList();
            

            return View();
        }

        public ActionResult CloneTask(int TaskId)
        {
            habiticatravelEntities MyHabitica = new habiticatravelEntities();
            DefaultTask TaskToClone = MyHabitica.DefaultTasks.Find(TaskId);
            ApplicationUser MyUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            CustomTask ClonedTask = new CustomTask
            {
                TaskName = TaskToClone.TaskName,
                TaskType = TaskToClone.TaskType,
                TaskTag = TaskToClone.TaskTag,
                TaskNotes = TaskToClone.TaskNotes,
                TaskDueDate = TaskToClone.TaskDueDate,
                TaskDifficulty = TaskToClone.TaskDifficulty,
                ReminderId = TaskToClone.ReminderId,
                ReminderStartDate = TaskToClone.ReminderStartDate,
                ReminderTime = TaskToClone.ReminderTime,
                TaskFrequency = TaskToClone.TaskFrequency,
                TaskRepeat = TaskToClone.TaskRepeat,
                TaskStreak = TaskToClone.TaskStreak,
                TaskStartDate = TaskToClone.TaskStartDate,
                TaskHabitUp = TaskToClone.TaskHabitUp,
                TaskHabitDown = TaskToClone.TaskHabitDown,
                TaskRewardValue = TaskToClone.TaskRewardValue,
                EveryXDays = TaskToClone.EveryXDays,
                UserId = MyUser.Id
            };

            MyHabitica.CustomTasks.Add(ClonedTask);
            MyHabitica.SaveChanges();

            List<DefaultTaskItem> CloneItemsList = new List<DefaultTaskItem>(MyHabitica.DefaultTasks.Find(TaskId).DefaultTaskItems.ToList());
            List<CustomTaskItem> NewItemsList = new List<CustomTaskItem>();
            
            for (int i = 0; i < CloneItemsList.Count; i++)
            {
                CustomTaskItem TempItem = new CustomTaskItem
                {
                    ItemName = CloneItemsList[i].ItemName,
                    TaskId = MyHabitica.CustomTasks.Find(ClonedTask).TaskId
                };
                MyHabitica.CustomTaskItems.Add(TempItem);
            }
            MyHabitica.SaveChanges();

            return View();
        }
    }
}