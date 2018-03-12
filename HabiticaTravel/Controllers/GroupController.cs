using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HabiticaTravel.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace HabiticaTravel.Controllers
{
    public class GroupController : Controller
    {
        // GET: Group
        public ActionResult ManageMyGroup()
        {
            var MyORM = new habiticatravelEntities();

            var userId = User.Identity.GetUserId();

            ViewBag.GroupUsers = MyORM.TravelGroupUsers.ToList();

            ViewBag.CurrentUser = userId;

            return View();
        }

        public ActionResult DisplayGroupScoreboard() // Essentially our index View
        {
            var MyORM = new habiticatravelEntities();

            ViewBag.GroupUsers = MyORM.TravelGroupUsers.ToList();

            return View();
        }

        public ActionResult SearchUserByEmail(string Email)
        {
            var HabiticaORM = new habiticatravelEntities();
            
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            ApplicationUser UserEmail = userManager.FindByEmail(Email);

            ViewBag.ShowEmailList = UserEmail;

            return View();
        }

        public ActionResult AddNewTravelGroup() // Adds new travel group
        {
            var model = new TravelGroup();
            return View(model);
        }

        public ActionResult SaveNewGroup(TravelGroup model)
        {
            var MyORM = new habiticatravelEntities();

            MyORM.TravelGroups.Add(model);

            MyORM.SaveChanges();

            return RedirectToAction("ManageMyGroup");
        }


        public ActionResult AddNewUserToGroup(TravelGroupUser model) // Adds new user to travel group
        {
            //1. Search user by email or username
            var HabiticaORM = new habiticatravelEntities();
            var userId = User.Identity.GetUserId();
            var NewGroupUser = HabiticaORM.TravelGroupUsers.Where(u => u.UserId == model.UserId).ToList();

            //2. Add member to group , we really might not need this.
            foreach (var GroupUser in NewGroupUser)
            {
                HabiticaORM.TravelGroupUsers.Add(GroupUser);
            }

            HabiticaORM.SaveChanges();

            //3. Return/Redirect Action to a View
            return View();
        }

        public ActionResult GetGroupToUpdate(int TravelGroupId)
        {
            var MyORM = new habiticatravelEntities();

            var CurrentTravelGroup = MyORM.TravelGroups.Find(TravelGroupId);

            TravelGroup GroupToBeEdited = MyORM.TravelGroups.Find(TravelGroupId);

            ViewBag.GroupUsers = CurrentTravelGroup;

            return View(); // create a view for this.
        }

        public ActionResult SaveUpdatedGroupChanges(TravelGroup newGroup)
        {
            var MyORM = new habiticatravelEntities();

            int TravelGroupId = newGroup.TravelGroupId;

            if (!ModelState.IsValid)
            {
                return View("../Shared/Error");
            }

            MyORM.Entry(MyORM.TravelGroups.Find(newGroup.TravelGroupId)).CurrentValues.SetValues(newGroup);

            MyORM.SaveChanges();


            return View();
        }

        public ActionResult DeleteGroupUser(int TravelGroupId, int TravelGroupUsersId)
        {
            var MyORM = new habiticatravelEntities();

            var ThegroupID = MyORM.TravelGroups.Where(tg => tg.TravelGroupId == TravelGroupId).FirstOrDefault();
            var GroupUsers = MyORM.TravelGroupUsers.Where(tg => tg.TravelGroupUsersId == TravelGroupUsersId).ToList();

            if (GroupUsers.Count != 0)
            {
                foreach (var gUser in GroupUsers)
                {
                    MyORM.TravelGroupUsers.Remove(gUser);
                }

            }
            MyORM.TravelGroups.Remove(ThegroupID);
            MyORM.SaveChanges();

            return RedirectToAction("Index", "Home");

        }
    }
}