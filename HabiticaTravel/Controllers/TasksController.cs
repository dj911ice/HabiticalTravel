﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HabiticaTravel.Models;
using HabiticaTravel.ViewModel;
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
                UserId = MyUser.Id,
                CustomTaskItems = new List<CustomTaskItem>()
            };

            MyHabitica.CustomTasks.Add(ClonedTask);
            try
            {
                MyHabitica.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }

            List<DefaultTaskItem> CloneItemsList = new List<DefaultTaskItem>(MyHabitica.DefaultTasks.Find(TaskId).DefaultTaskItems.ToList());
            List<CustomTaskItem> NewItemsList = new List<CustomTaskItem>();

            for (int i = 0; i < CloneItemsList.Count; i++)
            {
                var TempItem = new CustomTaskItem();
                int TempId = ClonedTask.TaskId;
                CustomTask TempTask = MyHabitica.CustomTasks.Find(TempId);
                TempItem.ItemName = CloneItemsList[i].ItemName;
                TempItem.TaskId = TempTask.TaskId;

                MyHabitica.CustomTaskItems.Add(TempItem);
                ClonedTask.CustomTaskItems.Add(TempItem);
            }

            try
            {
                MyHabitica.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult AddCustomTask(TaskAndItems Custom)
        {
            var HabiticaORM = new habiticatravelEntities();
            // HabiticaORM.CustomTasks.Add(Custom.CustomTask);
            HabiticaORM.SaveChanges();

            // HabiticaORM.CustomTaskItems.Add(Custom.CustomTaskItem);
            HabiticaORM.SaveChanges();

            return View();
        }
        public ActionResult CustomTask()
        {
            return View();
        }
        public ActionResult GetCustomTaskBYTaskId(int TaskId)
        {
            var HabiticaORM = new habiticatravelEntities();

            CustomTask TaskToEdit = HabiticaORM.CustomTasks.Find(TaskId);

            ViewBag.TaskToBeEdited = TaskToEdit;

            return View();
        }

        
        public ActionResult DeleteCustomTask(int TaskId)
        {
            var HabiticaORM = new habiticatravelEntities();

            HabiticaORM.CustomTasks.Remove(HabiticaORM.CustomTasks.Find(TaskId));
            HabiticaORM.SaveChanges();

            return View();
        }



        public ActionResult SaveCustomTaskChanges(CustomTask NewTask)
        {

            var HabiticaORM = new habiticatravelEntities();


            HabiticaORM.Entry(HabiticaORM.CustomTasks.Find(NewTask.TaskId)).CurrentValues.SetValues(NewTask);

            HabiticaORM.SaveChanges();

            return View();

        }


        public ActionResult ViewTask(int TaskId)
        {
            // going to find the task based on the task id , display all the task info into a view.
            // pk is TaskID , store that task in an object. Store it in a view bag. and send the viewbag to the view 

            var HabiticaORM = new habiticatravelEntities();

            ViewBag.task = HabiticaORM.CustomTasks.Find(TaskId);


            return View();
        
            

        }
    }
}