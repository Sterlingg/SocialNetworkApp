using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SocialNetworkApp.Models;
using WebMatrix.WebData;
using SocialNetworkApp.Filters;

namespace SocialNetworkApp.Controllers
{
    public class PostingController : Controller
    {

        private ISocialContext db;
        private IUserRepository ur;
        private IGroupRepository gr;

         public PostingController()
             : this(ContextHelper.GetContext(),new UserRepository(), new GroupRepository())
         { }

        /*
         *  Name: PostingController
         *  Description: Constructer that allows the context, UserRepository, and GroupRepository to be injected.                   
         */
        public PostingController(ISocialContext sc,IUserRepository userrepo, IGroupRepository grouprepo)
        {
            db = sc;        
            ur = userrepo;
            gr = grouprepo;
        }


        //
        // GET: /Posting/

        public ActionResult Index()
        {
            return View();
        }

    /* 
     * Name: _MakePost
     * Description: Returns the posting window view along with the groups the user 
     *              should see in the group selection drop down box based on
     *              if a group is public, or they are the owner of the group.
     * Arguments: None.              
     * View: /Posting/_MakePost             
     */
        [HttpGet]
        public ActionResult _MakePost()
        {
            User u = ur.GetUser();
            // take groups which are not private unioned with private groups to which the user is an owner
           // var glist = gr.NotPrivateOrOwner(u);
           List<Group> glist = gr.NotPrivateOrOwner(u).ToList();
           //For unit testing ease.
           ViewBag.TestingGroupList = glist;
           ViewBag.GroupList = new SelectList(glist, "GroupID", "GroupName");

            return PartialView("_MakePost");
        }

        /* 
          * Name: _MakePost
          * Description: Closes the posting window along with adding the 
         *               information filled out in the posting window to the 
         *               database.
          * Arguments: The PostLocationModel passed by the post form.             
          * View: /Posting/_MakePost             
          */
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), HttpPost]
        public ActionResult _MakePost(PostLocationModel plm)
        {
            User u = ur.GetUser();
            if (ModelState.IsValid)
            {

                plm.Tpost.UserID = u.UserId;

                //Location loc = new Location();
               // loc.LocationName = plm.Tlocation.LocationName;
               // loc.Longitude = plm.Tlocation.Longitude;
               // loc.Latitude = plm.Tlocation.Latitude;

                db.Locations.Add(plm.Tlocation);
                db.SaveChanges();

                int newPK = plm.Tlocation.LocationID;
                plm.Tpost.LocationID = newPK;
                plm.Tpost.CreationDate = System.DateTime.Now;

                db.Posts.Add(plm.Tpost);
                db.SaveChanges();

                var glist = gr.NotPrivateOrOwner(u);
                ViewBag.TestingGroupList = glist;
                ViewBag.GroupList = new SelectList(glist, "GroupID", "GroupName");

                //Close the post window, remove the circle selector, and add the new post
                // to the map.                 
                string closepostjs = "$('#postcontainer').hide(); removeRadiusSelctor();"
                                       + "updateMarkers();";

                return JavaScript(closepostjs);
            }

            // take groups which are not private unioned with private groups to which the user is an owner
            var glist1 = gr.NotPrivateOrOwner(u);
            ViewBag.TestingGroupList = glist1;
            ViewBag.GroupList = new SelectList(glist1, "GroupID", "GroupName");

            return PartialView("_MakePost");
        }

    }
}
