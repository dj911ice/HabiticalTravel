using HabiticaTravel.Models;
using HabiticaTravel.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
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

        public async Task<ActionResult> AddCustomTask(TaskAndItems model)
        {

            var currentUser = System.Web.HttpContext.Current
                .GetOwinContext()
                .GetUserManager<ApplicationUserManager>()
                .FindById(System.Web.HttpContext
                .Current.User.Identity.GetUserId());

            var HabiticaORM = new habiticatravelEntities();
            model.CustomTask.UserId = currentUser.Id;

            HabiticaORM.CustomTask.Add(model.CustomTask);
            await HabiticaORM.SaveChangesAsync();

            var currentTask = HabiticaORM.CustomTask.Where(t => model.CustomTask.TaskName == t.TaskName).FirstOrDefault();
            var taskItems = model.CustomTaskItem.ToList();
            foreach (var item in taskItems)
            {
                item.TaskId = currentTask.TaskId;
            }
            currentTask.CustomTaskItems = taskItems;

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
            var selectedTaskItems = HabiticaORM.CustomTaskItem.Where(t => t.TaskId == TaskId).ToList();

            foreach (var item in selectedTaskItems)
            {
                HabiticaORM.CustomTaskItem.Remove(item);

            }

            HabiticaORM.CustomTask.Remove(selectedTask);

            HabiticaORM.SaveChanges();

            return RedirectToAction("Index", "Home");
        }



        public ActionResult SaveCustomTaskChanges(TaskAndItems NewTaskAndItems)
        {

            var HabiticaORM = new habiticatravelEntities();

            int TaskId = NewTaskAndItems.CustomTask.TaskId;

            CustomTask DBTask = HabiticaORM.CustomTasks.Find(TaskId);

            List<CustomTaskItem> DBItemsList = new List<CustomTaskItem>();
            if(HabiticaORM.CustomTasks.Find(TaskId).CustomTaskItems != null)
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

            return RedirectToAction("Index");

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