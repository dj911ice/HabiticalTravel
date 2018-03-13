using HabiticaTravel.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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

        public ActionResult DisplayGroupScoreboard() // Essentially our index View
        {
            var MyORM = new habiticatravelEntities();

            ViewBag.GroupUsers = MyORM.TravelGroupUsers.ToList();

            return View();
        }

        public ActionResult UserSearch()
        {
            return View();
        }

        public ActionResult SearchUserByEmail(string Email)
        {
            var HabiticaORM = new habiticatravelEntities();


            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            var user = userManager.FindByEmail(Email);

            var userEmail = user.Email.ToList();


            if (user == null)
            {
                // if user does not exist redirect to page where you enter the email

                RedirectToAction("UserSearch");

                // store into viewbag error "user does not exist"
                ViewBag.UserNullMessage = ("Sorry, Please enter an email of a registered user");
            }
            else
            {
                ViewBag.ShowEmailList = userEmail;
            }

            return View("UserSearch");
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

        public ActionResult DeleteGroupUser(int TravelGroupUsersId)
        {
            var MyORM = new habiticatravelEntities();

            
            var currentUser = MyORM.TravelGroupUsers.Find(TravelGroupUsersId);

            MyORM.TravelGroupUsers.Remove(currentUser);
            MyORM.SaveChanges();

            return RedirectToAction("ManageMyGroup");

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
    }
}