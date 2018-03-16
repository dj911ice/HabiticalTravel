using HabiticaTravel.Models;
using HabiticaTravel.Utility;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace HabiticaTravel.Controllers
{
    [Authorize]
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
            HabiticaUser MyHabUser = MyHabitica.HabiticaUsers.Single(u => u.UserId == userId);

            CustomTask ClonedTask = new CustomTask
            {
                TaskName = TaskToClone.TaskName,
                TaskType = TaskToClone.TaskType,
                TaskTag = MyHabUser.TaskTagId,
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
            List<CustomTaskItem> items;

            if (model.CustomTaskItem != null)
            {
                items = model.CustomTaskItem;
                HabiticaORM.CustomTaskItems.AddRange(items);
            }
            else
            {
                items = new List<CustomTaskItem> { new CustomTaskItem() };
                HabiticaORM.CustomTaskItems.AddRange(items);
            }


            model.CustomTask.UserId = userId;
            HabiticaUser MyHabUser = HabiticaORM.HabiticaUsers.Single(u => u.UserId == userId);
            model.CustomTask.TaskTag = MyHabUser.TaskTagId;
            model.CustomTask.TaskType = "todo";
            model.CustomTaskItem = items;
            var TaskConfirm = (JObject)JObject.FromObject(await HabiticaHTTP.PostNewHabiticaTask(model.CustomTask, MyHabUser));

            HabiticaORM.CustomTasks.Add(model.CustomTask);

            try
            {
                HabiticaORM.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
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

            var currentTask = HabiticaORM.CustomTasks.Where(t => model.CustomTask.TaskId == t.TaskId).FirstOrDefault();
            var TestItem = (string)TaskConfirm["data"]["id"];
            currentTask.HabiticaTaskId = (string)TaskConfirm["data"]["id"];
            string habtaskid = currentTask.HabiticaTaskId;

            HabiticaORM.SaveChanges();

            if (model.CustomTask.CustomTaskItems.Count != 0)
            {
                var taskItems = model.CustomTaskItem.ToList();
                foreach (var item in taskItems)
                {
                    var ItemConfirm = (JObject)JObject.FromObject(await HabiticaHTTP.PostNewChecklistItem(item, MyHabUser, currentTask));
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

            return View(TaskAndItemToEdit); //TaskAndItemToEdit is sent to EditCustomTask View which returns to SaveCustomTaskChanges
        }

        public async Task<ActionResult> RemoveTask(int TaskId)
        {
            var HabiticaORM = new habiticatravelEntities();
            string UserId = User.Identity.GetUserId();
            HabiticaUser MyHabUser = HabiticaORM.HabiticaUsers.Single(u => u.UserId == UserId);

            var selectedTask = HabiticaORM.CustomTasks.Where(t => t.TaskId == TaskId).FirstOrDefault();
            var selectedTaskItems = HabiticaORM.CustomTaskItems.Where(t => t.TaskId == TaskId).ToList();

            if (selectedTaskItems.Count != 0)
            {
                foreach (var item in selectedTaskItems)
                {
                    var ItemConfirm = (JObject)JObject.FromObject(await HabiticaHTTP.DeleteChecklistItem(selectedTask, item, MyHabUser));
                    HabiticaORM.CustomTaskItems.Remove(item);
                }
            }
            var ItemConfirm2 = (JObject)JObject.FromObject(await HabiticaHTTP.DeleteATask(selectedTask, MyHabUser));
            HabiticaORM.CustomTasks.Remove(selectedTask);
            HabiticaORM.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> SaveCustomTaskChanges(TaskAndItems NewTaskAndItems)
        {

            var HabiticaORM = new habiticatravelEntities();
            string UserId = User.Identity.GetUserId();
            HabiticaUser MyHabUser = HabiticaORM.HabiticaUsers.Single(u => u.UserId == UserId);

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
                    var ItemConfirm = (JObject)JObject.FromObject(await HabiticaHTTP.PutUpdateChecklistItem(T, MyHabUser, MyTask));
                }
            }
            MyTask.CustomTaskItems = MyItemsList;
            HabiticaORM.Entry(DBTask).CurrentValues.SetValues(MyTask);
            var ItemConfirm2 = (JObject)JObject.FromObject(await HabiticaHTTP.PutUpdateTask(MyTask, MyHabUser));
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
            List<CustomTaskItem> MyItemsList = HabiticaORM.CustomTaskItems.Where(t => t.TaskId == TaskId).ToList();
            TaskAndItems model = new TaskAndItems();
            model.CustomTask = HabiticaORM.CustomTasks.Find(TaskId);
            model.CustomTaskItem = MyItemsList;

            ViewBag.yelp = yelpResults;

            return View(model);
        }

        public ActionResult CreateNewGroupCustomTask(int TravelGroupId)
        {
            var MyHabiticaORM = new habiticatravelEntities();
            var MyUsers = new List<TravelGroupUser>();
            var model = new TravelGroupandUserTaskandItems()
            {
                Users = MyHabiticaORM.TravelGroupUsers.Where(u => u.TravelGroupId == TravelGroupId).ToList(),
                TravelGroupandUser = new TravelGroupandUser()
                {
                    TravelGroup = MyHabiticaORM.TravelGroups.Find(TravelGroupId),
                },

            };

            // model.TravelGroupandUser.TravelGroup = MyHabitica.TravelGroups.Find(TravelGroupId)

            return View("GroupCustomTasks", model);
        }

        public ActionResult AddGroupCustomTask(TravelGroupandUserTaskandItems model)
        {
            var HabiticaORM = new habiticatravelEntities();
            model.TaskAndItems.CustomTask.TravelGroupId = model.TravelGroupandUser.TravelGroup.TravelGroupId;
            HabiticaORM.CustomTasks.Add(model.TaskAndItems.CustomTask);
            HabiticaORM.SaveChanges();

            TempData["taskAndItems"] = model.TaskAndItems;
            TempData["travelGroupandUser"] = model.TravelGroupandUser;

            return RedirectToAction("CloneGroupCustomTaskForUsers");
        }

        public async Task<ActionResult> CloneGroupCustomTaskForUsers()
        {
            var taskAndItems = (TaskAndItems)TempData["taskAndItems"];
            var travelGroupandUser = (TravelGroupandUser)TempData["travelGroupandUser"];
            var HabiticaORM = new habiticatravelEntities();
            var travelGroupUsers = HabiticaORM.TravelGroupUsers.Where(u => u.TravelGroupId == travelGroupandUser.TravelGroup.TravelGroupId).ToList();

            foreach (var user in travelGroupUsers)
            {
                taskAndItems.CustomTask.UserId = user.UserId;
                var MyHabUser = HabiticaORM.HabiticaUsers.Single(u => u.UserId == user.UserId);
                taskAndItems.CustomTask.TaskTag = MyHabUser.TaskTagId;

                HabiticaORM.CustomTasks.Add(taskAndItems.CustomTask);
                HabiticaORM.SaveChanges();

                var TaskConfirm = JObject.FromObject(await HabiticaHTTP.PostNewHabiticaTask(taskAndItems.CustomTask, MyHabUser));

                // assigning the taskId from habitica to the current task we are working with. we are doing this so
                // when we submit this task back to habitica, users will have updated exp and stats.
                taskAndItems.CustomTask.HabiticaTaskId = (string)TaskConfirm["data"]["id"];

                if (taskAndItems.CustomTaskItem != null)
                {
                    foreach (var item in taskAndItems.CustomTaskItem)
                    {
                        // we are adding each checklist item id from habiticas api and storing it in the current
                        // task that we are working with, then we are adding each of those task items into the database
                        // which again contains that checklist id. we are doing this so we can submit this task
                        // back to habitica so we can updat their exp.
                        var ItemConfirm = (JObject)JObject.FromObject(await HabiticaHTTP.PostNewChecklistItem(item, MyHabUser, taskAndItems.CustomTask));
                        List<Checklist> AllChecklistItems = ItemConfirm["data"]["checklist"].ToObject<List<Checklist>>();
                        foreach (Checklist checkItem in AllChecklistItems)
                        {
                            if (checkItem.text == item.ItemName)
                            {
                                item.HabiticaItemId = checkItem.id;
                            }
                        }

                        item.TaskId = taskAndItems.CustomTask.TaskId;
                        HabiticaORM.CustomTaskItems.Add(item);
                    }

                }
                else

                {
                    taskAndItems.CustomTask.CustomTaskItems = new List<CustomTaskItem>();
                }
                HabiticaORM.SaveChanges();
            }
            var route = new RouteValueDictionary(new { controller = "Tasks", action = "ShowGroupTasks", travelGroupId = travelGroupandUser.TravelGroup.TravelGroupId });
            return RedirectToAction("ShowGroupTasks", route);
        }

        public ActionResult ShowGroupTasks(int travelGroupId)
        {
            // we need the travelGroupID to find the current group we are working with.
            var MyHabiticaORM = new habiticatravelEntities();
            var model = new TravelGroupandUserTaskandItems
            {
                ManyTaskAndItemsList = new List<TaskAndItems>(),
                Users = MyHabiticaORM.TravelGroupUsers.Where(u => u.TravelGroupId == travelGroupId).ToList(),
                TravelGroupandUser = new TravelGroupandUser
                {
                    TravelGroup = MyHabiticaORM.TravelGroups.Find(travelGroupId),
                },

            };
            var tasks = MyHabiticaORM.CustomTasks.Where(ct => ct.TravelGroupId == travelGroupId && ct.UserId == null).ToList();
            foreach (CustomTask task in tasks)
            {
                var tempTaskItems = new TaskAndItems
                {
                    CustomTask = task
                };
                model.ManyTaskAndItemsList.Add(tempTaskItems);
            }

            return View(model);
        }

        public async Task<ActionResult> RemoveGroupTask(TravelGroupandUserTaskandItems model, int TaskId)
        {
            habiticatravelEntities HabiticaORM = new habiticatravelEntities();
            CustomTask selectedTask = HabiticaORM.CustomTasks.Single(t => t.TaskId == TaskId && t.UserId == null);
            var selectedTaskItems = HabiticaORM.CustomTaskItems.Where(t => t.TaskId == TaskId).ToList();

            if (selectedTaskItems.Count != 0)
            {
                foreach (var item in selectedTaskItems)
                {
                    HabiticaORM.CustomTaskItems.Remove(item);
                }

            }
            HabiticaORM.CustomTasks.Remove(selectedTask);


            List<TravelGroupUser> GroupUsers = new List<TravelGroupUser>();
            GroupUsers = HabiticaORM.TravelGroupUsers.Where(u => u.TravelGroupId == model.TravelGroupandUser.TravelGroup.TravelGroupId).ToList();

            foreach (TravelGroupUser user in GroupUsers)
            {
                HabiticaUser MyHabUser = HabiticaORM.HabiticaUsers.Single(u => u.UserId == user.UserId);
                CustomTask selectedUserTask = HabiticaORM.CustomTasks.Single(t => t.TravelGroupId == model.TaskAndItems.CustomTask.TravelGroupId && t.UserId == user.UserId && t.TaskName == model.TaskAndItems.CustomTask.TaskName);
                var selectedUserTaskItems = HabiticaORM.CustomTaskItems.Where(t => t.TaskId == selectedUserTask.TaskId).ToList();

                if (selectedUserTaskItems.Count != 0)
                {
                    foreach (var item in selectedUserTaskItems)
                    {
                        var ItemConfirm = (JObject)JObject.FromObject(await HabiticaHTTP.DeleteChecklistItem(selectedUserTask, item, MyHabUser));
                        HabiticaORM.CustomTaskItems.Remove(item);
                    }

                }
                var ItemConfirm2 = (JObject)JObject.FromObject(await HabiticaHTTP.DeleteATask(selectedUserTask, MyHabUser));
                HabiticaORM.CustomTasks.Remove(selectedUserTask);
            }

            HabiticaORM.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult EditGroupCustomTask(TravelGroupandUserTaskandItems model, int TaskId)

        {
            habiticatravelEntities HabiticaORM = new habiticatravelEntities();

            var CurrentTaskItems = new List<CustomTaskItem>(HabiticaORM.CustomTasks.Find(TaskId).CustomTaskItems.ToList());

            CustomTask TaskToEdit = HabiticaORM.CustomTasks.Find(TaskId);

            TaskAndItems TaskAndItemToEdit = new TaskAndItems
            {
                CustomTask = TaskToEdit,
                CustomTaskItem = CurrentTaskItems

            };


            ViewBag.TaskToBeEdited = TaskToEdit;

            return View(TaskAndItemToEdit);
        }

        public async Task<ActionResult> SaveCustomGroupTaskChanges(TaskAndItems NewTaskAndItems)
        {

            habiticatravelEntities HabiticaORM = new habiticatravelEntities();

            int TaskId = NewTaskAndItems.CustomTask.TaskId;

            CustomTask DBTask = HabiticaORM.CustomTasks.Find(TaskId);

            List<CustomTaskItem> DBItemsList = new List<CustomTaskItem>();
            if (DBTask.CustomTaskItems != null)
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

            //Above changes the Group Item only. Below we change the values for each Group Member


            List<TravelGroupUser> GroupUsers = new List<TravelGroupUser>();
            GroupUsers = HabiticaORM.TravelGroupUsers.Where(u => u.TravelGroupId == NewTaskAndItems.CustomTask.TravelGroupId).ToList();

            foreach (TravelGroupUser user in GroupUsers)
            {
                HabiticaUser MyHabUser = HabiticaORM.HabiticaUsers.Single(u => u.UserId == user.UserId);
                CustomTask UserTask = HabiticaORM.CustomTasks.Single(u => u.TravelGroupId == NewTaskAndItems.CustomTask.TravelGroupId && u.UserId == user.UserId && u.TaskName == DBTask.TaskName);

                List<CustomTaskItem> UserItemsList = new List<CustomTaskItem>();
                if (UserTask.CustomTaskItems != null)
                {
                    UserItemsList = HabiticaORM.CustomTasks.Find(UserTask.TaskId).CustomTaskItems.ToList();
                }

                CustomTask MyUserTask = NewTaskAndItems.CustomTask;
                List<CustomTaskItem> MyUserItemsList = new List<CustomTaskItem>();

                if (NewTaskAndItems.CustomTaskItem != null && DBItemsList.Count != 0)
                {
                    foreach (CustomTaskItem T in NewTaskAndItems.CustomTaskItem)
                    {
                        MyUserItemsList.Add(T);
                        HabiticaORM.Entry(HabiticaORM.CustomTaskItems.Find(T.TaskItemsId)).CurrentValues.SetValues(T);
                        var ItemConfirm = (JObject)JObject.FromObject(await HabiticaHTTP.PutUpdateChecklistItem(T, MyHabUser, MyTask));
                    }
                }
                MyUserTask.CustomTaskItems = MyUserItemsList;
                HabiticaORM.Entry(UserTask).CurrentValues.SetValues(MyUserTask);
                var ItemConfirm2 = (JObject)JObject.FromObject(await HabiticaHTTP.PutUpdateTask(MyTask, MyHabUser));
                HabiticaORM.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }
    }
}