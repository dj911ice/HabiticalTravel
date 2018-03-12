using HabiticaTravel.Models;
using HabiticaTravel.Utility;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace HabiticaTravel.Controllers.Habitica
{
    public partial class HabiticaAccountController : Controller
    {

        // THESE PROPERTIES AND FIELDS ARE SUPER IMPORTANT, THEY ARE HERE TO ALLOW US TO REGISTER
        // A NEW USER IF THEY ALREADY HAVE AN ACCOUNT WITH HABITICA, I COPIED THIS CODE ESSENTIALLY
        // FROM THE ORIGINAL ACCOUNT FROM IDENTY AND IM JUST USING IT FOR A SINGLE PURPOSE.
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public HabiticaAccountController()
        {
        }

        public HabiticaAccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private UserManager<ApplicationUser> userManager;
        // ANYTHING ABOVE THIS IS AGAIN PART OF THE FIELDS/PROPERTIES ORIGINALY FROM ACCOUNT IDENTITY INFORMATION.

        // GET: HabiticaAccount

        public ActionResult HabiticaLogin()
        {

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> HabiticaLoginHandler(RegisterViewModel model)
        {
            var JSON = (JObject)JObject.FromObject(await HabiticaHTTP.PostUserLogin(model.Email, model.Password));
            if (bool.Parse(JSON["success"].ToString()))
            {
                var UpdatedHabiticaUser = new HabiticaUser
                {
                    Uuid = (string)JSON["data"]["id"],
                    ApiToken = (string)JSON["data"]["apiToken"],
                };

                var HabiticaORM = new habiticatravelEntities();
                userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

                // This is for two purposes, if this returns null, than we make a new user,
                // if it does not, we will find this user in the Habitica table by ID.
                ApplicationUser User = userManager.FindByEmail(model.Email);
                if (User == null)
                {
                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                    var result = await UserManager.CreateAsync(user, model.Password);
                    
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        var newUser = userManager.FindByEmail(user.Email);
                        UpdatedHabiticaUser.UserId = newUser.Id;

                        // pulling user data so we can check if that user has the HabiticaAbroad tag, reason we care
                        // is we do not want to create duplicate tags on their habitica account. 
                        var userData = (JObject)JObject.FromObject(await HabiticaHTTP.GetUserData(UpdatedHabiticaUser));
                        List<Tag> tags = userData["data"]["tags"].ToObject<List<Tag>>();
                        // var isTagged = tags.Where(t => t.id == "Habitica Abroad");
                        var tagKey = JObject.FromObject(await HabiticaHTTP.PostCreateTag(UpdatedHabiticaUser));
                        UpdatedHabiticaUser.TaskTagId = (string)tagKey["data"]["id"];
                        HabiticaORM.HabiticaUsers.Add(UpdatedHabiticaUser);
                        HabiticaORM.SaveChanges();
                        return RedirectToAction("Index", "Home");
                    }
                    AddErrors(result);

                    return View(model);
                }
                else
                {
                    // simply updating the habitica User that is not null with the
                    // HabiticaUser object that was created on line 88
                    var result = await SignInManager.PasswordSignInAsync(User.UserName, model.Password, false, shouldLockout: false);
                    switch (result)
                    {
                        case SignInStatus.Success:
                            return RedirectToAction("Index", "Home");
                        case SignInStatus.Failure:
                        default:
                            ModelState.AddModelError("", "Invalid login attempt.");
                            return View(model);
                    }
                }
            }
            else
            {
                // this is just a place holder, we need to pass a view that tells user 
                // that 
                ViewBag.Message = "You do not have an account with Habitica. Please Register.";
                return View("../Habitica/AlreadyRegisteredWithHabitica");
            }

        }

        public ActionResult HabiticaUserForm()
        {
            return View();
        }

        public async Task<ActionResult> RegisterNewUser()
        {
            // test
            var JSON = (JObject)TempData["JSON"];
            var user = (ApplicationUser)TempData["user"];
            
            // these are both of our ORM copies, so the the first one is our Habitica User,
            // second ORM is basicaly the an object that wraps around our ApplicationUser, the reason
            // we have to do it this way is because UserManager allows us to preform CRUD operation
            // to our Identity database. 
            var HabiticaORM = new habiticatravelEntities();
            userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            // simply finding the user in our Identity database by whatever the user put into
            // the User Name form field. 
            var idenUser = userManager.FindByName(user.UserName);

            // creating a new HabiticaUser object which will store the required Non-NULL values, we did
            // not include the data for HabiticaUserID because that will auto increment in the database.
            var habiticaUser = new HabiticaUser
            {
                ApiToken = (string)JSON["data"]["apiToken"],
                Uuid = (string)JSON["data"]["id"],
                UserId = idenUser.Id
            };

            var tagKey = (JObject)JObject.FromObject(await HabiticaHTTP.PostCreateTag(habiticaUser));
            habiticaUser.TaskTagId = (string)tagKey["data"]["id"];
            // finishing off our CRUD operation, we are adding the new HabiticaUser into our database
            // and saving our changes so it wil reflect in the database.  then we are simply returning 
            // the HomePage view for now. 
            HabiticaORM.HabiticaUsers.Add(habiticaUser);
            HabiticaORM.SaveChanges();
            return RedirectToAction("Index", "Home");

        }

    }
}