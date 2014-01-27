// ***********************************************************************
// Assembly         : SocialNetworkApp
// Author           : Team Valdes
// Created          : 11-16-2012
//
// Last Modified By : Team Valdes
// Last Modified On : 11-16-2012
// ***********************************************************************
// <copyright file="SideBarController.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SocialNetworkApp.Models;
using System.Globalization;


namespace SocialNetworkApp.Controllers
{
    public class SideBarController : Controller
    {
        private SocialContext db = new SocialContext();
        private UserRepository ur = new UserRepository();
        private GroupRepository gr = new GroupRepository();

        //
        // GET: /SideBar/
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        // GET: /SideBar/_SideBar
        [HttpGet]
        public ActionResult _SideBar()
        {
            return PartialView();
        }

    //ADD GROUPS
        //Returned when a group is successfully made.
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public ActionResult GroupSuccess()
        {
            string sidebarjs = "alert('Group successfully added!');";

            return JavaScript(sidebarjs);
        }

        //GET: /SideBar/_AddGroup
        //Returns _AddGroup partial view.
        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public ActionResult _AddGroup()
        {
            return PartialView();
        }

        //POST: Add group to database.     
        // Post: /ModalForm/
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), Authorize(Roles = "Admin, User")]
        [HttpPost]
        public ActionResult _AddGroup(Group group)
        {
            if (ModelState.IsValid)
            {
                db.Groups.Add(group);
                db.SaveChanges();

                //add the creator of the group to the database
                ur.AddCreatedGroup(group.GroupID);
                ur.Save();

                ViewBag.GroupList = db.Groups.Select(x => new SelectListItem { Text = x.GroupName, Value = x.GroupID.ToString(CultureInfo.CurrentCulture) });
                return RedirectToAction("GroupSuccess");
            }
            ViewBag.GroupList = db.Groups.Select(x => new SelectListItem { Text = x.GroupName, Value = x.GroupID.ToString(CultureInfo.CurrentCulture) });
            return PartialView(group);
        }

    // SUBSCRIPTIONS
        // Helper Method: ChangeSubscription
        // Change the current users subscription status to a given group.
        //     GroupId: the group to change the subscription on
        //     addSubscription: if true, add subscription, if false remove subscription
        private void ChangeSubscription(int GroupId, bool addSubscription)
        {
            //user is adding a new subscription
            if (addSubscription)
            {
                ur.AddSubscription(GroupId);
            }
            else // user is removing a subscription
            {
                ur.RemoveSubscription(GroupId);
            }
            ur.Save();
        }


    // POPULAR GROUPS
        // GET: /SideBar/_PopularGroups
        // Displays a list of the five groups with most posts in the sidebar
        [HttpGet]
        public ActionResult _PopularGroups()
        {
            PopularGroupsModel pgm = new PopularGroupsModel();
            pgm.CurrentUser = ur.GetUser();
            pgm.Groups = gr.GetPopularGroups().ToList();
            return PartialView(pgm);
        }

        // POST: /SideBar/_ChangeSubscription
        // Change Subscription as called from _PopularGroups 
        [HttpPost]
        public PartialViewResult _ChangeSubscription(int GroupId, bool addSubscription)
        {
            ChangeSubscription(GroupId, addSubscription);
            PopularGroupsModel pgm = new PopularGroupsModel();
            pgm.CurrentUser = ur.GetUser();
            pgm.Groups = gr.GetPopularGroups().ToList();
            return PartialView("_PopularGroups", pgm);
        }

  
    // GROUP SEARCHES
        // GET: /SideBar/_SearchGroups
        // Display view for searching through public groups
        [HttpGet]
        public ActionResult _SearchGroups()
        {
            SearchGroupsModel sgm = new SearchGroupsModel();
            sgm.SearchTerm = "";
            return PartialView(sgm);
        }

        // POST: /SideBar/_SearchGroups
        // Perform a search and update the view
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), HttpPost]
        public ActionResult _SearchGroups(SearchGroupsModel sgm)
        {
            if (ModelState.IsValid)
            {
                //Changed to gr1 due to Static Analysis
                GroupRepository gr1 = new GroupRepository();
                sgm.Groups = gr1.SearchOnTerm(sgm.SearchTerm).ToList();
                return PartialView("_SearchGroups", sgm);
            }
            return PartialView("_SearchGroups", sgm);
        }

   // VIEW GROUP DATA

        // Helper: Populate ViewGroupDataModel
        private ViewGroupDataModel FillGroupDataModel(int gid)
        {
            User u = ur.GetUser();
            Group g = db.Groups.FirstOrDefault(k => k.GroupID == gid);
            ViewGroupDataModel vgdm = new ViewGroupDataModel();
            vgdm.CurrentUser = u;
            vgdm.Group = g;
            return vgdm;
        }
        // GET: /SideBar/_ViewGroupData
        // Show information about the given group, in respect to the current user.
        //      gid: the id of the group we are viewing
        [HttpGet]
        public ActionResult _ViewGroupData(int gid)
        {
            ViewBag.IsSubscribed = ur.IsSubscribedTo(gid);
            ViewBag.IsOwner = ur.IsOwnerOf(gid);
            return PartialView(FillGroupDataModel(gid));
        }

        // POST: /SideBar/_ChangeGroupSubscription
        // Change Subscription as called from _ViewGroupData
        [HttpPost]
        public PartialViewResult _ChangeGroupSubscription(int GroupId, bool addSubscription)
        {
            ChangeSubscription(GroupId, addSubscription);
            ViewBag.IsSubscribed = ur.IsSubscribedTo(GroupId);
            ViewBag.IsOwner = ur.IsOwnerOf(GroupId);
            return PartialView("_ViewGroupData", FillGroupDataModel(GroupId));
        }

    // EDIT GROUPS
        // GET: /SideBar/_EditGroupOptions
        // Show the moderator's view of a group, and editing options
        //         GroupId: id of the group we are editing
        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public ActionResult _EditGroupOptions(int GroupId)
        {
            if (ur.IsOwnerOf(GroupId))
            {
                EditGroupDataModel egdm = new EditGroupDataModel();
                Group g = db.Groups.FirstOrDefault(k => k.GroupID == GroupId);
                egdm.Group = g;
                egdm.Offset = 0;
                return PartialView(egdm);
            }
            return _ViewGroupData(GroupId);
        }

        // POST: /SideBar/_EditGroupRemovePost
        // Remove a post from the group we are currently editing in _EditGroupOptions
        //         egdm: state model
        //          PostId: the post to delete when called from _EditGroupRemovePostById
        [HttpPost]
        public PartialViewResult _EditGroupRemovePost(EditGroupDataModel egdm, int PostId = 0)
        {
            Group g = db.Groups.FirstOrDefault(k => k.GroupID == egdm.Group.GroupID);
            if (PostId == 0)
            {
                PostId = int.Parse(Request.Form[0]);
            }
            try
            {
                Post post = g.Posts.First(p => p.PostID == PostId);
                db.Posts.Remove(post);
                db.SaveChanges();
            }
            catch
            {
                ModelState.AddModelError("DeletingPostId", "No post with id \"" + PostId.ToString() + "\" exists in this group.");
            }
            egdm.Group = g;
            return PartialView("_EditGroupOptions", egdm);
        }

        // POST: /SideBar/_EditGroupRemovePostByID
        // Remove a post from the group in _EditGroupOptions, for a given PostID
        [HttpPost]
        public PartialViewResult _EditGroupRemovePostByID(EditGroupDataModel egdm)
        {
            return _EditGroupRemovePost(egdm, egdm.DeletingPostId);
        }

        // POST: /SideBar/_EditGroupScrollPosts
        // Scroll through posts in _EditGroupOptions
        //       increaseOffset: true, we are scrolling forward, false, scrolling back
        [HttpPost]
        public PartialViewResult _EditGroupScrollPosts(EditGroupDataModel egdm, string IncreaseOffset)
        {
            var offFlag = Boolean.Parse(IncreaseOffset);
            Group g = db.Groups.FirstOrDefault(k => k.GroupID == egdm.Group.GroupID);
            egdm.Group = g;
            int off = 0;
            int offset = int.Parse(Request.Form[0]);
            if (offFlag)
            {
                off = Math.Min(offset + 5, g.PostCount - 5);
            }
            else
            {
                off = Math.Max(offset - 5, 0);
            }
            egdm.Offset = off;
            return PartialView("_EditGroupOptions", egdm);
        }

        // POST: /SideBar/_EditGroupMakePublic
        // Make a private group public in _EditGroupOptions
        [HttpPost]
        public PartialViewResult _EditGroupMakePublic(EditGroupDataModel egdm)
        {
            var g = db.Groups.FirstOrDefault(k => k.GroupID == egdm.Group.GroupID);
            egdm.Group = g;
            g.IsPrivate = false;
            db.SaveChanges();
            return PartialView("_EditGroupOptions", egdm);
        }

        // POST: /SideBar/_EditGroupAddInvite
        // Invite a user to join/subscribe to the group in _EditGroupOptions
        //         InviteUserName: the invited user's name
        [HttpPost]
        public PartialViewResult _EditGroupAddInvite(EditGroupDataModel egdm, string InviteUserName)
        {
            Group g = db.Groups.FirstOrDefault(k => k.GroupID == egdm.Group.GroupID);
            try
            {
                User toinvite = db.Users.First(d => d.UserName == InviteUserName);
                if (g.Users2.Contains(toinvite)) // already subscribed
                {
                    ModelState.AddModelError("InviteUserName", "\"" + InviteUserName + "\"That user is already subscribed to the group.");
                }
                else if (g.Users1.Contains(toinvite)) // already invited to join
                {
                    ModelState.AddModelError("InviteUserName", "\"" + InviteUserName + "\" is already invited to the group.");
                }
                else
                {
                    ur.AllowUserSubscribeToGroup(toinvite.UserId, g.GroupID);
                    ur.Save();
                }
            }
            catch
            { // oops, that user does not exist!
                ModelState.AddModelError("InviteUserName", "No user by name \"" + InviteUserName + "\" exists");
            }
            egdm.Group = g;
            return PartialView("_EditGroupOptions", egdm);
        }

        // POST: /SideBar/_EditGroupAddMod
        // Allow a user to have moderator priveleges for the _EditGroupOptions group
        [HttpPost]
        public PartialViewResult _EditGroupAddMod(EditGroupDataModel egdm, string ModUserName)
        {
            Group g = db.Groups.FirstOrDefault(k => k.GroupID == egdm.Group.GroupID);
            try
            {
                User toinvite = db.Users.First(d => d.UserName == ModUserName);
                if (!g.Users2.Contains(toinvite)) // the user is not subscribed to the group and cannot be moderator
                {
                    ModelState.AddModelError("ModUserName", "\"" + ModUserName + "\" must be subscribed to this group to be a mod.");
                }
                else if (g.Users.Contains(toinvite)) // already a moderator!
                {
                    ModelState.AddModelError("ModUserName", "\"" + ModUserName + "\" is already a moderator for this group.");
                }
                else
                {
                    ur.GiveUserOwnership(toinvite.UserId, g.GroupID);
                    ur.Save();
                }
            }
            catch
            { // user does not exist!
                ModelState.AddModelError("ModUserName", "No user by name \"" + ModUserName + "\" exists");
            }
            egdm.Group = g;
            return PartialView("_EditGroupOptions", egdm);
        }

        // POST: /SideBar/_EditGroupBanUser
        // Remove a user's subscription to the group in _EditGroupOptions
        [HttpPost]
        public PartialViewResult _EditGroupBanUser(EditGroupDataModel egdm, string BanUserName)
        {
            Group g = db.Groups.FirstOrDefault(k => k.GroupID == egdm.Group.GroupID);
            try
            {
                User toinvite = db.Users.First(d => d.UserName == BanUserName);
                if (!g.Users2.Contains(toinvite))
                {
                    ModelState.AddModelError("BanUserName", "\"" + BanUserName + "\" is not subscribed to this group.");
                }
                else if (g.Users.Contains(toinvite)) // currently, moderators cannot be banned.
                {
                    ModelState.AddModelError("BanUserName", "\"" + BanUserName + "\" is a moderator for the group and cannot be banned.");
                }
                else
                {
                    ur.BanUserFromGroup(toinvite.UserId, g.GroupID);
                    ur.Save();
                }
            }
            catch
            {
                ModelState.AddModelError("BanUserName", "No user by name \"" + BanUserName + "\" exists");
            }
            egdm.Group = g;
            return PartialView("_EditGroupOptions", egdm);
        }

    // YOUR GROUPS PAGE
        // GET: /SideBar/_ViewYourGroups
        // Show a list of the current user's moderated groups, subscriptions, and invites.
        [HttpGet]
        public ActionResult _ViewYourGroups()
        {
            User u = ur.GetUser();
            var Subscriptions = ur.GetSubscriptions();
            var OwnedGroups = ur.GetOwnedGroups();
            var AllowedGroups = ur.GetInvitedGroups();
            ViewYourGroupsModel vygm = new ViewYourGroupsModel();
            vygm.CurrentUser = u;
            vygm.Invitations = AllowedGroups.ToList();
            vygm.Subscriptions = Subscriptions.ToList();
            vygm.Ownerships = OwnedGroups.ToList();
            return PartialView(vygm);
        }
    
    // PROFILE PAGE
        // Helper method FillProfileModel
        // Create a new ProfileModel from the currentuser
        //      currentoffset: offset in the post table in the profile
        private ProfileModel FillProfileModel(int currentoffset = 0)
        {
            ProfileModel pm = new ProfileModel();
            User u = ur.GetUser();
            var written = ur.GetCreatedPosts();         
            var blocked = ur.GetBlockedUsers();         
            var blockposts = ur.GetBlockedPosts();
            pm.CurrentUser = u;
            pm.WrittenPosts = written.ToList();
            pm.Blocks = blocked.ToList();
            pm.BlockedPosts = blockposts.ToList();
            pm.Offset = currentoffset;
            pm.UserToBlock = "";
            return pm;
        }

        // GET: /SideBar/_ViewProfile
        // Show the profile for the current user
        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public ActionResult _ViewProfile()
        {
            return PartialView(FillProfileModel());
        }

        // POST: /SideBar/_ProfileRemovePost
        // Delete a post as chosen by the user in _ViewProfile
        [HttpPost]
        public PartialViewResult _ProfileRemovePost(ProfileModel pm, int CurrOffset)
        {
            int PostId = int.Parse(Request.Form[0]);
            try
            {
                Post post = db.Posts.First(p => p.PostID == PostId);
                db.Posts.Remove(post);
                db.SaveChanges();
            }
            catch
            {
                ModelState.AddModelError("DeletingPostId", "No post with id \"" + PostId.ToString() + "\" exists.");
            }
            return PartialView("_ViewProfile", FillProfileModel(CurrOffset));
        }

        // POST: /SideBar/_ProfileScrollPosts
        // Scroll through posts table in _ViewProfile
        [HttpPost]
        public PartialViewResult _ProfileScrollPosts(ProfileModel pm, string IncreaseOffset)
        {
            var written = ur.GetCreatedPosts();
            var offFlag = Boolean.Parse(IncreaseOffset);
            int off = 0;
            int offset = int.Parse(Request.Form[0]);
            if (offFlag)
            {
                off = Math.Min(offset + 5, written.Count() - 5); 
            }
            else
            {
                off = Math.Max(offset - 5, 0); 
            }
            return PartialView("_ViewProfile", FillProfileModel(off));
        }

        // POST: /SideBar/_ProfileManageBlocks
        // Block or unblock another user from _ViewProfile
        //      BlockName: The user to be blocked/unbocked
        //      CurrOffSet: offset in the post table, just keeping track of it
        //      IsBlocking: if true, the user will be blocked,
        //                  if false, the user is being unblocked.
        [HttpPost]
        public PartialViewResult _ProfileManageBlocks(string BlockName, int CurrOffset, bool IsBlocking)
        {
            if (IsBlocking)
            {
                ur.BlockUser(BlockName);
            }
            else
            {
                ur.UnBlockUser(BlockName);   
            }
            ur.Save();
            return PartialView("_ViewProfile", FillProfileModel(CurrOffset));
        }

        // POST: /SideBar/_ProfileUnblockUser
        // Unblock a user from _ViewProfile
        [HttpPost]
        public PartialViewResult _ProfileUnblockUser(string BlockName, int CurrOffset)
        {
            return _ProfileManageBlocks(BlockName, CurrOffset, false);
        }

        // POST: /SideBar/_ProfileBlockUser
        // Block a user from _ViewProfile
        [HttpPost]
        public PartialViewResult _ProfileBlockuser(ProfileModel pm, int CurrOffset)
        {
            User u = ur.GetUser();
            try
            {
                if (pm.UserToBlock == "" || pm.UserToBlock == null)
                    ModelState.AddModelError("UserToBlock", "You must enter a valid username.");
                else if (pm.UserToBlock == u.UserName)
                    ModelState.AddModelError("UserToBlock", "You cannot block yourself!");
                else
                {
                    User b = db.Users.First(d => d.UserName == pm.UserToBlock);
                    if (u.User1.Contains(b))
                        ModelState.AddModelError("UserToBlock", "You are already blocking \"" + pm.UserToBlock + "\"");
                    else
                        return _ProfileManageBlocks(pm.UserToBlock, CurrOffset, true);
                }
            }
            catch
            {
                ModelState.AddModelError("UserToBlock", "No user by name \"" + pm.UserToBlock + "\" exists.");
            }
            return PartialView("_ViewProfile", FillProfileModel(CurrOffset));
        }

        // POST: /SideBar/_ProfileChangeDescription
        // Change the current users description from _ViewProfile
        [HttpPost]
        public PartialViewResult _ProfileChangeDescription(ProfileModel pm)
        {
            ur.ChangeDescription(pm.CurrentUser.ProfileDescription);
            ur.Save();
            return PartialView("_ViewProfile", FillProfileModel(pm.Offset));
        }

    }
}

