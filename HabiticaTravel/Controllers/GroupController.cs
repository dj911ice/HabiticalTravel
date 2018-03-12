using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HabiticaTravel.Models;
using Microsoft.AspNet.Identity;

namespace HabiticaTravel.Controllers
{
    public class GroupController : Controller
    {
        // GET: Group
        public ActionResult DisplayGroup() // Essentially our index View
        {
            var MyORM = new habiticatravelEntities();

            ViewBag.GroupUsers = MyORM.TravelGroupUsers.ToList();

            return View();
        }


        public ActionResult AddNewTravelGroup(TravelGroup TGroupId) // Adds new travel group
        {
            var HabiticaORM = new habiticatravelEntities();
            var NewTravelGroup = HabiticaORM.TravelGroups.Where(tgi => tgi.TravelGroupId == TGroupId.TravelGroupId).ToList();

            foreach (var TravelGroup in NewTravelGroup)
            {
                HabiticaORM.TravelGroups.Add(TravelGroup);
            }

            HabiticaORM.SaveChanges();

            return View("DisplayGroup");
        }

        public ActionResult AddNewUserToGroup(TravelGroupUser NewUser) // Adds new user to travel group
        {
            //1. Search user by email or username
            var HabiticaORM = new habiticatravelEntities();
            var userId = User.Identity.GetUserId();
            var NewGroupUser = HabiticaORM.TravelGroupUsers.Where(u => u.UserId == NewUser.UserId).ToList();

            //2. Add member to group
            foreach (var GroupUser in NewGroupUser)
            {
                HabiticaORM.TravelGroupUsers.Add(GroupUser);
            }

            HabiticaORM.SaveChanges();

            //3. Return/Redirect Action to a View
            return View("DisplayGroup");
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