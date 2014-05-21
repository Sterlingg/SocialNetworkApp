using SocialNetworkApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SocialNetworkApp.Controllers
{
    public class FooterController : Controller
    {
        private IUserRepository ur;
         //Added to import the ability to test users in the app.

        public FooterController()
            : this(new UserRepository())
        {
        }
        public FooterController(IUserRepository userrepo)
        {
            ur = userrepo;
        }

        [HttpGet]
        public ActionResult _Subscriptions()
        {
            //Not used according to Static Analysis
            //User u = ur.GetUser();
            var grouplist = ur.GetSubscriptions().ToList();
            return PartialView("_Subscriptions", grouplist);
        }

    }

}
