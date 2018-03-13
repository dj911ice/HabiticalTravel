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
            string userId = User.Identity.GetUserId();
            CustomTask ClonedTask = new CustomTask
            {
                TaskName = TaskToClone.TaskName,
                TaskType = TaskToClone.TaskType,
                TaskTag = TaskToClone.TaskTag,
                TaskNotes = TaskToClone.TaskNotes,
                TaskDueDate = TaskToClone.TaskDueDate,
                TaskDifficulty = TaskToClone.TaskDifficulty,
                ReminderStartDate = TaskToClone.ReminderStartDate,
                ReminderTime = TaskToClone.ReminderTime,
                UserId = userId,
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

        public async Task<ActionResult> AddCustomTask(TaskAndItems model)
        {

            string userId = User.Identity.GetUserId();

            var HabiticaORM = new habiticatravelEntities();
            model.CustomTask.UserId = userId;

            HabiticaORM.CustomTasks.Add(model.CustomTask);
            await HabiticaORM.SaveChangesAsync();

            HabiticaUser MyHabUser = HabiticaORM.HabiticaUsers.Single(u => u.UserId == userId);
            var TaskConfirm =  (JObject)JObject.FromObject(await HabiticaHTTP.PostNewHabiticaTask(model.CustomTask, MyHabUser));

            var currentTask = HabiticaORM.CustomTasks.Where(t => model.CustomTask.TaskId == t.TaskId).FirstOrDefault();
            var TestItem = (string)TaskConfirm["data"]["id"];
            currentTask.HabiticaTaskId = (string)TaskConfirm["data"]["id"];

            if (model.CustomTask.CustomTaskItems.Count != 0)
            {
                var taskItems = model.CustomTaskItem.ToList();
                foreach (var item in taskItems)
                {
                    var ItemConfirm = (JObject)JObject.FromObject(await HabiticaHTTP.PostNewChecklistItem(item, MyHabUser));
                    List<Checklist> AllChecklistItems = ItemConfirm["data"]["checklist"].ToObject<List<Checklist>>();
                    foreach(Checklist list in AllChecklistItems)
                    {
                        if(list.text == item.ItemName)
                        {
                            item.HabiticaItemId = list.id;
                        }
                    }

                    item.TaskId = currentTask.TaskId;
                }
                currentTask.CustomTaskItems = taskItems;
            }
            else

            {
                currentTask.CustomTaskItems = new List<CustomTaskItem>();
            }
            

            HabiticaORM.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult CustomTask()
        {
            return View();
        }

        public ActionResult EditCustomTask(int TaskId)

        {
            var HabiticaORM = new habiticatravelEntities();

            var currentTaskItems = new List<CustomTaskItem>(HabiticaORM.CustomTasks.Find(TaskId).CustomTaskItems.ToList());

            CustomTask TaskToEdit = HabiticaORM.CustomTasks.Find(TaskId);

            TaskAndItems TaskAndItemToEdit = new TaskAndItems
            {
                CustomTask = TaskToEdit,
                CustomTaskItem = currentTaskItems

            };


            ViewBag.TaskToBeEdited = TaskToEdit;

            return View(TaskAndItemToEdit);
        }

        public ActionResult RemoveTask(int TaskId)
        {
            var HabiticaORM = new habiticatravelEntities();

            var selectedTask = HabiticaORM.CustomTasks.Where(t => t.TaskId == TaskId).FirstOrDefault();
            var selectedTaskItems = HabiticaORM.CustomTaskItems.Where(t => t.TaskId == TaskId).ToList();

            if (selectedTaskItems.Count != 0)
            {
                foreach (var item in selectedTaskItems)
                {
                    HabiticaORM.CustomTaskItems.Remove(item);
                }

            }
            HabiticaORM.CustomTasks.Remove(selectedTask);
            HabiticaORM.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult SaveCustomTaskChanges(TaskAndItems NewTaskAndItems)
        {

            var HabiticaORM = new habiticatravelEntities();

            int TaskId = NewTaskAndItems.CustomTask.TaskId;

            CustomTask DBTask = HabiticaORM.CustomTasks.Find(TaskId);

            List<CustomTaskItem> DBItemsList = new List<CustomTaskItem>();
            if (HabiticaORM.CustomTasks.Find(TaskId).CustomTaskItems != null)
            {
                DBItemsList = HabiticaORM.CustomTasks.Find(TaskId).CustomTaskItems.ToList();
            }

            CustomTask MyTask = NewTaskAndItems.CustomTask;
            List<CustomTaskItem> MyItemsList = new List<CustomTaskItem>();

            if (NewTaskAndItems.CustomTaskItem != null && DBItemsList.Count != 0)
            {
                foreach (CustomTaskItem T in NewTaskAndItems.CustomTaskItem)
                {
                    MyItemsList.Add(T);
                    HabiticaORM.Entry(HabiticaORM.CustomTaskItems.Find(T.TaskItemsId)).CurrentValues.SetValues(T);
                }
            }
            MyTask.CustomTaskItems = MyItemsList;
            HabiticaORM.Entry(DBTask).CurrentValues.SetValues(MyTask);
            HabiticaORM.SaveChanges();

            return RedirectToAction("Index", "Home");

        }

        public async Task<ActionResult> ViewTask(int TaskId)
        {
            // going to find the task based on the task id , display all the task info into a view.
            // pk is TaskID , store that task in an object. Store it in a view bag. and send the viewbag to the view 
            var yelpSearch = new YelpSearchHTTP();
            var yelpResults = new List<JObject>();
            var cats = new YelpCat
            {
                Hotel = "hotels, All",
                Restaurant = "restaurants, All",
                Arts = "arts, All",
                Grocery = "grocery, All",
            };

            yelpResults.Add(JObject.FromObject(await yelpSearch.GetResults("77373", cats.Hotel)));
            yelpResults.Add(JObject.FromObject(await yelpSearch.GetResults("77373", cats.Restaurant)));
            yelpResults.Add(JObject.FromObject(await yelpSearch.GetResults("77373", cats.Grocery)));
            yelpResults.Add(JObject.FromObject(await yelpSearch.GetResults("77373", cats.Arts)));

            yelpResults = yelpResults.Select(y => y = (JObject)y["businesses"][0]).ToList();

            var HabiticaORM = new habiticatravelEntities();
                
            ViewBag.task = HabiticaORM.CustomTasks.Find(TaskId);
            ViewBag.yelp = yelpResults;
           
            return View();
        }

        public async Task<ActionResult> AddGroupCustomTask(TaskAndItems model, TravelGroupandUsers model2)
        {

            string userId = User.Identity.GetUserId();

            var HabiticaORM = new habiticatravelEntities();
            model.CustomTask.UserId = userId;

            HabiticaORM.CustomTasks.Add(model.CustomTask);
            await HabiticaORM.SaveChangesAsync();

            HabiticaUser MyHabUser = HabiticaORM.HabiticaUsers.Single(u => u.UserId == userId);
            var TaskConfirm = (JObject)JObject.FromObject(await HabiticaHTTP.PostNewHabiticaTask(model.CustomTask, MyHabUser));

            var currentTask = HabiticaORM.CustomTasks.Where(t => model.CustomTask.TaskId == t.TaskId).FirstOrDefault();
            var TestItem = (string)TaskConfirm["data"]["id"];
            currentTask.HabiticaTaskId = (string)TaskConfirm["data"]["id"];

            if (model.CustomTask.CustomTaskItems.Count != 0)
            {
                var taskItems = model.CustomTaskItem.ToList();
                foreach (var item in taskItems)
                {
                    var ItemConfirm = (JObject)JObject.FromObject(await HabiticaHTTP.PostNewChecklistItem(item, MyHabUser));
                    List<Checklist> AllChecklistItems = ItemConfirm["data"]["checklist"].ToObject<List<Checklist>>();
                    foreach (Checklist list in AllChecklistItems)
                    {
                        if (list.text == item.ItemName)
                        {
                            item.HabiticaItemId = list.id;
                        }
                    }

                    item.TaskId = currentTask.TaskId;
                }
                currentTask.CustomTaskItems = taskItems;
            }
            else

            {
                currentTask.CustomTaskItems = new List<CustomTaskItem>();
            }


            HabiticaORM.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

    }
}