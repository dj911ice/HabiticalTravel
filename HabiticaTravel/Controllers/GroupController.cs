using HabiticaTravel.Models;
using HabiticaTravel.Utility;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace HabiticaTravel.Controllers
{
    public class GroupController : Controller
    {
        // GET: Group

        public ActionResult ManageMyGroup()
        {
            var MyORM = new habiticatravelEntities();
            var currentUserId = User.Identity.GetUserId();


            // UserGroups viewmodel to pass both TravelGroup and the current username
            var currentTravelGroups = MyORM.TravelGroups.Where(g => g.GroupLeader == currentUserId).ToList();

            List<TravelGroupandUser> model = new List<TravelGroupandUser>();

            // we need to store a list of TravelGroupandUsers, I modified the viewmodel to also contain user id so we 
            // can conditionally render the buttons according to if the user owns the group.
            foreach (var travelGroup in currentTravelGroups)
            {
                model.Add(new TravelGroupandUser()
                {
                    TravelGroup = new TravelGroupVM()
                    {
                        Destination = travelGroup.Destination,
                        GroupLeader = travelGroup.GroupLeader,
                        TravelGroupId = travelGroup.TravelGroupId,
                        TravelMethod = travelGroup.TravelMethod
                    },
                    UserName = User.Identity.GetUserName(),
                });
            }

            ViewBag.CurrentUser = currentUserId;

            // passing the current UserId so that only the current user will see CRUD
            // operation buttons. ie, the group leader.
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
            var model = new TravelGroupVM();
            return View(model);
        }

        public ActionResult SaveNewGroup(TravelGroupVM model)
        {
            var MyORM = new habiticatravelEntities();

            // GroupLeader is related to the Users ID, the ID that exists in Identity
            var userId = User.Identity.GetUserId();

            MyORM.TravelGroups.Add(new TravelGroup()
            {
                Destination = model.Destination,
                TravelGroupName = model.TravelGroupName,
                TravelMethod = model.TravelMethod,
                GroupLeader = userId
            });

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

        public ActionResult UserSearch(TravelGroupandUser model)
        {
            ViewBag.GroupId = model.TravelGroup.TravelGroupId;
            return View();
        }

        public ActionResult UserSearchByEmailForm(TravelGroupandUser model)
        {
            return View();
        }

        public ActionResult SearchUserByEmail(string Email, TravelGroupandUser model)
        {

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            //var userEmail
            try
            {
                ApplicationUser user = userManager.FindByEmail(Email);
                ViewBag.showEmail = user.Email;
                model.TravelGroupUser.UserId = user.Id;
               // model.TravelGroupUser = MyTGUser;
                return View("UserSearchByEmailForm", model);
            }
            catch (NullReferenceException)
            {
                // store into viewbag error "user does not exist"
                ViewBag.UserNullMessage = ("Sorry, Please enter an email of a registered user");
                return View("UserSearchByEmailForm");
            }

        }

        public ActionResult AddNewUserToGroup(TravelGroupandUser model) // Adds new user to travel group
        {
            //1. Search user by email or username
            var HabiticaORM = new habiticatravelEntities();

            TravelGroupUser MyTravelGroupUser = new TravelGroupUser()
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

        public ActionResult DeleteGroupUser(TravelGroupandUser model)
        {
            var MyORM = new habiticatravelEntities();

            TravelGroupUser UserToDelete = MyORM.TravelGroupUsers.Find(model.TravelGroupUsers.TravelGroupUsersId);

            MyORM.TravelGroupUsers.Remove(UserToDelete);
            MyORM.SaveChanges();

            return RedirectToAction("ManageMyGroup");

        }

    }
}