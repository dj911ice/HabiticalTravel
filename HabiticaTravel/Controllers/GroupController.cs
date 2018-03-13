using HabiticaTravel.Models;
using HabiticaTravel.Utility;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HabiticaTravel.Controllers
{
    public class GroupController : Controller
    {
        // GET: Group

        public ActionResult ManageMyGroup()
        {
            var MyORM = new habiticatravelEntities();
            var userId = User.Identity.GetUserId();
            var test = new TravelGroup();
            var model = new UsersGroups
            {
                TravelGroups = MyORM.TravelGroups.Where(g => g.GroupLeader == userId).ToList(),
                UserName = User.Identity.GetUserName()
            };
            ViewBag.CurrentUser = userId;

            return View(model);
        }

        public ActionResult DisplayGroupScoreboard(int TravelGroupId) // Essentially our index View
        {
            var MyORM = new habiticatravelEntities();

            var Group = MyORM.TravelGroups.Find(TravelGroupId);

            List<TravelGroupUser> GroupUsers = Group.TravelGroupUsers.ToList();
            List<GroupUserAndName> model = new List<GroupUserAndName>();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            foreach (var user in GroupUsers)
            {
                model.Add(new GroupUserAndName
                {
                    TGUser = user,
                    UserName = userManager.FindById(user.UserId).UserName
                });
            }
            
            return View(model);
        }

        public ActionResult AddNewTravelGroup() // Adds new travel group
        {
            var model = new TravelGroup();
            return View(model);
        }

        public ActionResult SaveNewGroup(TravelGroup model)
        {
            var MyORM = new habiticatravelEntities();

            var userId = User.Identity.GetUserId();
            model.GroupLeader = userId;

            MyORM.TravelGroups.Add(model);
            MyORM.SaveChanges();

            var selectedGroup = MyORM.TravelGroups.Where(tg => tg.TravelGroupName == model.TravelGroupName).FirstOrDefault();
            
            var tUser = new TravelGroupUser
            {
                TravelGroupId = selectedGroup.TravelGroupId,
                UserId = userId,
                UserGroupRole = Convert.ToBoolean((int)GroupRole.Leader),
                UserGroupScore = 0
            };

            MyORM.TravelGroupUsers.Add(tUser);
            MyORM.SaveChanges();

            return RedirectToAction("ManageMyGroup");
        }



        public ActionResult AddNewUserToGroup(TravelGroupandUsers model, int userrole, int score) // Adds new user to travel group
        {
            //1. Search user by email or username
            var HabiticaORM = new habiticatravelEntities();
            
            TravelGroupUser MyTravelGroupUser = new TravelGroupUser
            {
                UserId = model.TravelGroupUser.UserId,
                TravelGroupId = model.TravelGroup.TravelGroupId,
                UserGroupRole = false,
                UserGroupScore = score
            };
            //2. Add member to group , we really might not need this.
            
            HabiticaORM.TravelGroupUsers.Add(MyTravelGroupUser);
            
            HabiticaORM.SaveChanges();

            //3. Return/Redirect Action to a View
            return View();
        }

        public ActionResult UpdateGroup(int TravelGroupId)
        {
            var MyORM = new habiticatravelEntities();

            var model = MyORM.TravelGroups.Find(TravelGroupId);

            return View(model); // create a view for this.
        }

        public ActionResult SaveUpdatedGroupChanges(TravelGroup model)
        {
            var MyORM = new habiticatravelEntities();

            int TravelGroupId = model.TravelGroupId;

            if (!ModelState.IsValid)
            {
                return View("../Shared/Error");
            }

            MyORM.Entry(MyORM.TravelGroups.Find(model.TravelGroupId)).CurrentValues.SetValues(model);

            MyORM.SaveChanges();

            return View("ManageMyGroup");
        }

        public ActionResult DeleteGroup(int travelGroupID)
        {
            var MyORM = new habiticatravelEntities();

            var CurrentGroup = MyORM.TravelGroups.Find(travelGroupID);

            List<TravelGroupUser> GroupUsers = MyORM.TravelGroupUsers.Where(gU => gU.TravelGroupId == travelGroupID).ToList();
            if (GroupUsers.Count != 0)
            {

                foreach (var gUser in GroupUsers)
                {

                    MyORM.TravelGroupUsers.Remove(gUser);
                }

            }
            MyORM.TravelGroups.Remove(CurrentGroup);
            MyORM.SaveChanges();


            return RedirectToAction("ManageMyGroup");
        }

        public ActionResult UserSearch(TravelGroupandUsers model)
        {
            ViewBag.GroupId = model.TravelGroup.TravelGroupId;
            return View();
        }

        public ActionResult SearchUserByEmail(string Email, TravelGroupandUsers model)
        {

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            //var userEmail
            try
            {
                ApplicationUser user = userManager.FindByEmail(Email);
                ViewBag.ShowEmailList = user.Email;
                TravelGroupUser MyTGUser = new TravelGroupUser
                {
                    UserId = user.Id,
                };
                model.TravelGroupUser = MyTGUser;
                return View("UserSearch", model);
            }
            catch (NullReferenceException)
            {
                // store into viewbag error "user does not exist"
                ViewBag.UserNullMessage = ("Sorry, Please enter an email of a registered user");
                return View("UserSearch");
            }
           
        }

        public ActionResult AddNewUserToGroup(TravelGroupandUsers model) // Adds new user to travel group
        {
            //1. Search user by email or username
            var HabiticaORM = new habiticatravelEntities();
            
            TravelGroupUser MyTravelGroupUser = new TravelGroupUser
            {
                UserId = model.TravelGroupUser.UserId,
                TravelGroupId = model.TravelGroup.TravelGroupId,
                UserGroupRole = false,
                UserGroupScore = 0
            };
            //2. Add member to group , we really might not need this.
            
            HabiticaORM.TravelGroupUsers.Add(MyTravelGroupUser);
            
            HabiticaORM.SaveChanges();

            //3. Return/Redirect Action to a View
            return View();
        }

        public ActionResult DeleteGroupUser(TravelGroupandUsers model)
        {
            var MyORM = new habiticatravelEntities();

            TravelGroupUser UserToDelete = MyORM.TravelGroupUsers.Find(model.TravelGroupUser.TravelGroupUsersId);
            
            MyORM.TravelGroupUsers.Remove(UserToDelete);
            MyORM.SaveChanges();

            return RedirectToAction("ManageMyGroup");

        }

    }
}