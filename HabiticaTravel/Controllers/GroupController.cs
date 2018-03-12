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

            ViewBag.GroupUsers = MyORM.UserGroupScores.ToList();


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