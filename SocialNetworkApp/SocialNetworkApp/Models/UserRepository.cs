// ***********************************************************************
// Assembly         : SocialNetworkApp
// Author           : Team Valdes
// Created          : 11-16-2012
//
// Last Modified By : Team Valdes
// Last Modified On : 11-16-2012
// ***********************************************************************
// <copyright file="UserRepository.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary>
//  A repository for database access methods related to users.    
// </summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMatrix.WebData;

namespace SocialNetworkApp.Models
{

    public class UserRepository : SocialNetworkApp.Models.IUserRepository
    {
        private ISocialContext db;
        private IPostRepository pr;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private IGroupRepository gr;
        private IWebSecurity WebSecurity { get; set; }

        /*
         *  Name: UserRepository
         *  Description: Set up the controller for web deployment with a
         *               concrete SocialContext, WebSecurity, PostRepository
         *               and GroupRepository.
         *  Arguments: None.              
         */
        public UserRepository()
            : this(ContextHelper.GetContext(), new WebSecurityWrapper(), new PostRepository(), new GroupRepository())
        { }

        /*
         *  Name: UserRepository
         *  Description: Constructor to allow ease of mocking
         *  Arguments: Implementations of the context,security, and repositories.
         */
        public UserRepository(ISocialContext sc, IWebSecurity webSecurity
                              , IPostRepository postrepo, IGroupRepository grouprepo)
        {
            db = sc;
            WebSecurity = webSecurity;
            pr = postrepo;
            gr = grouprepo;
        }


        /* 
        * Name: GetPostsLoggedIn
        * Description: Returns posts filtered by what the user is subscribed to
        *              and the posts within range of their current location.
        * Arguments: The user's latitude and the users longitude.              
        */
        public IQueryable GetPostsLoggedIn(string lat, string lon)
        {
            User curruser = GetUser();
            var BlockedUsers = GetBlockedUsers();
            IQueryable<Group> UsersGroups = GetSubscriptions();
            IQueryable<Post> PostsInProximity = pr.FindNearbyPosts(decimal.Parse(lat), decimal.Parse(lon));
            IQueryable<Location> locations = db.Locations;
            return from t in
                       (from p in PostsInProximity
                        from g in UsersGroups
                        where (g.GroupID == p.GroupID)
                        where !BlockedUsers.Contains(p.User)
                        select new
                        {
                            Content = p.PostContent,
                            Title = p.PostTitle,
                            GroupID = p.GroupID,
                            LocationID = p.LocationID

                        })
                   from l in locations
                   where l.LocationID == t.LocationID
                   select new
                   {
                       Latitude = l.Latitude,
                       Longitude = l.Longitude,
                       Content = t.Content,
                       Title = t.Title,
                       GroupID = t.GroupID
                   };

        }

        /* 
         * Name: GetPostsNotLoggedIn
         * Description: Returns posts filtered by what the user is subscribed to     
         * Arguments: The user's latitude and the users longitude.              
         */
        public IQueryable GetPostsNotLoggedIn(string lat, string lon)
        {
            IQueryable<Location> locations = db.Locations;
            IQueryable<Post> posts = db.Posts;
            IQueryable<Group> groups = db.Groups;
            return (
             from l in locations
             from p in posts
             from g in groups
             where (l.LocationID == p.LocationID) && (g.GroupID == p.GroupID)
                   && (g.IsPrivate == false)    //&& (p.EndDate.HasValue && (p.EndDate < today))                     
             select new
             {
                 Latitude = l.Latitude,
                 Longitude = l.Longitude,
                 Content = p.PostContent,
                 Title = p.PostTitle,
                 GroupID = p.GroupID
             });

        }

        /*
         * Name: GetUser
         * Description: Returns the currently logged in user, or null if no
         *              user is logged in.
         * Arguments: None.             
         */
        public User GetUser()
        {
            int curruser = WebSecurity.CurrentUserId();
            return db.Users.FirstOrDefault(d => d.UserProfileId == curruser);
        }

        /*
         * Name: GetUser
         * Description: Returns a user based on the id given.
         * Arguments: The desired user's profile id.             
         */
        public User GetUser(int userProfileId)
        {
            return db.Users.FirstOrDefault(d => d.UserProfileId == userProfileId);
        }

        /*
         * Name: GetSubscriptions
         * Description: Returns the groups that the current user is subscribed to.
         * Arguments: None.           
         */
        public IQueryable<Group> GetSubscriptions()
        {
            User CurrUser = GetUser();
            int uid = CurrUser.UserId;

            return (from gr in db.Groups
                    from u in gr.Users2
                    where (u.UserId == uid)
                    select gr);
        }

        /*
         * Name: GetOwnedGroups
         * Description: Returns the groups that the current user owns.
         * Arguments: None.           
         */
        public IQueryable<Group> GetOwnedGroups()
        {
            User CurrUser = GetUser();
            int uid = CurrUser.UserId;
            return (from gr in db.Groups
                    from u in gr.Users
                    where (u.UserId == uid)
                    select gr);

        }

        /*
         * Name: GetInvitedGroups
         * Description: Returns the groups that the current user is invited to.
         * Arguments: None.           
         */
        public IQueryable<Group> GetInvitedGroups()
        {
            User CurrUser = GetUser();
            int uid = CurrUser.UserId;
            return from gr in db.Groups
                   from u in gr.Users1
                   where (u.UserId == uid)
                   select gr;
        }

        /*
         * Name: IsSubscribedTo
         * Description: Returns true if the user is subscribe to the given group
         *              and false otherwise.
         * Arguments: The desired groups id.          
         */
        public bool IsSubscribedTo(int gid)
        {
            var subscriptions = GetSubscriptions().ToList();
            try
            {
                Group g = db.Groups.First(d => d.GroupID == gid);
                if (subscriptions.Contains(g))
                {
                    return true;
                }
            }
            catch { }
            return false;

        }

        /*
         * Name: IsOwnerOf
         * Description: Returns true if the user is the owner of the given group
         *              and false otherwise.
         * Arguments: The desired groups id.          
         */
        public bool IsOwnerOf(int gid)
        {
            var ownerships = GetOwnedGroups().ToList();
            try
            {
                Group g = db.Groups.First(d => d.GroupID == gid);
                if (ownerships.Contains(g))
                {
                    return true;
                }
            }
            catch { }
            return false;
        }

        /*
         * Name: GetBlockedUsers
         * Description: Returns the users that the current user has blocked.
         * Arguments: None.           
         */
        public IQueryable<User> GetBlockedUsers()
        {
            User curruser = GetUser();
            return from u in db.Users
                   from t in u.Users
                   where t.UserId == curruser.UserId
                   select u;
        }

        /*
         * Name: GetBlockedPosts
         * Description: Returns the posts that the current user has blocked.
         * Arguments: None.           
         */
        public IQueryable<Post> GetBlockedPosts()
        {
            User curruser = GetUser();
            return from u in db.Posts
                   from p in u.Users
                   where p.UserId == curruser.UserId
                   select u;
        }

        /*
         * Name: GetCreatedPosts
         * Description: Returns the posts that the current user has created.
         * Arguments: None.           
         */
        public IQueryable<Post> GetCreatedPosts()
        {
            User curruser = GetUser();
            return from u in db.Posts
                   where u.UserID == curruser.UserId
                   select u;
        }

        /*
         * Name: AddCreatedGroup
         * Description: Adds the creator of a group to the group database.
         * Arguments: The group id of the group being created.          
         */
        public void AddCreatedGroup(int groupId)
        {
            User curruser = GetUser();
            try
            {
                Group g = db.Groups.Single(d => d.GroupID == groupId);
                if (!g.Users.Contains(curruser))
                {
                    g.Users.Add(curruser);
                }
            }
            catch { }
        }

        /*
         * Name: RemoveCreatedGroup
         * Description: Removes a group from the database.
         * Arguments: The group id of the group being removed.         
         */
        public void RemoveCreatedGroup(int groupId)
        {
            User curruser = GetUser();
            try
            {
                Group g = db.Groups.Single(d => d.GroupID == groupId);
                if (g.Users.Contains(curruser))
                {
                    if (g.IsPrivate)
                    {
                        // g is not the only owner of a private group
                        if (g.Users.Count() > 1)
                        {
                            g.Users.Remove(curruser);
                        }
                        else
                        {
                            //TODO: Exception alerting user that if the groups is deleted than everything in the group is deleted
                        }
                    }
                    else
                    {
                        g.Users.Remove(curruser); //Public Groups can have no creators.
                    }
                }
            }
            catch { }
        }

        /*
         * Name: AddSubscription
         * Description: Subscribes the current user to the group given in the argument.
         * Arguments: The group id of the group being subscribed to.          
         */
        public void AddSubscription(int groupId)
        {
            User u = GetUser();
            try
            {
                Group g = db.Groups.Single(d => d.GroupID == groupId);
                //not allowed to join a private group and is not an owner for the group
                if (g.IsPrivate && !g.Users1.Contains(u) && !g.Users.Contains(u))
                {
                    return;
                }

                if (g.Users1.Contains(u)) // has been invited to join group, remove them from invite list
                {
                    g.Users1.Remove(u);
                }
                if (!g.Users2.Contains(u))
                    g.Users2.Add(u);
            }
            catch { }
        }

        /*
         * Name: RemoveSubscription
         * Description: Unsubscribes the current user from the group given in the argument.
         * Arguments: The group id of the group being unsubscribed from.          
         */
        public void RemoveSubscription(int groupId)
        {
            User u = GetUser();
            try
            {
                Group g = db.Groups.Single(d => d.GroupID == groupId);
                if (g.Users2.Contains(u))
                    g.Users2.Remove(u);
            }
            catch { }
            // Question: If group is private, and you remove subscription, 
            // should you still be on the "allowed users" list?
            // (currently not implemented)
        }

        /*
         * Name: BlockUser
         * Description: Adds a user to the current users blocked list given
         *              the blockee's user name.
         * Arguments: The name of the user to block.         
         */
        public void BlockUser(string blockUserName)
        {
            User u = GetUser();
            try
            {
                User toBlock = db.Users.First(b => b.UserName == blockUserName);
                if (!u.User1.Contains(toBlock))
                    u.User1.Add(toBlock);
            }
            catch { }
        }

        /*
         * Name: UnBlockUser
         * Description: Removes a user from the current users blocked list given
         *              the blockee's user name.
         * Arguments: The name of the user to unblock.         
         */
        public void UnBlockUser(string unblockUserName)
        {
            User u = GetUser();
            try
            {
                User toUnBlock = db.Users.First(b => b.UserName == unblockUserName);
                if (u.User1.Contains(toUnBlock))
                    u.User1.Remove(toUnBlock);
            }
            catch { }
        }

        /*
         * Name: BlockPost
         * Description: Adds a post to the current user's blocked post list.              
         * Arguments: The id of the post to block.
         */
        public void BlockPost(int blockId)
        {
            User u = GetUser();
            try
            {
                Post toBlock = db.Posts.Single(d => d.PostID == blockId);
                if (!u.Posts1.Contains(toBlock))
                    u.Posts1.Add(toBlock);
            }
            catch { }
        }

        /*
         * Name: UnBlockPost
         * Description: Removes a post from the current user's blocked post list.              
         * Arguments: The id of the post to unblock.
         */
        public void UnBlockPost(int blockedId)
        {
            User u = GetUser();
            try
            {
                Post toUnBlock = db.Posts.Single(d => d.PostID == blockedId);
                if (u.Posts1.Contains(toUnBlock))
                    u.Posts1.Remove(toUnBlock);
            }
            catch { }
        }

        /*
         * Name: AllowUserSubscribeToGroup
         * Description: Checks whether a user is already subscribed to a group/           
         * Arguments: The id of the user to add to the group given in groupId.
         */
        public void AllowUserSubscribeToGroup(int allowedUserId, int groupId)
        {
            User u = GetUser();
            try
            {
                User allowed = db.Users.Single(d => d.UserId == allowedUserId);
                Group g = db.Groups.Single(d => d.GroupID == groupId);

                // check if user 'u' is an owner of group g
                if (g.Users.Contains(u))
                {
                    // user is already subscribed, or is an owner of the group
                    if (g.Users2.Contains(allowed) || g.Users.Contains(allowed))
                        return;
                    // check if user 'allowed' is not already allowed to join
                    if (!g.Users1.Contains(allowed))
                    {
                        g.Users1.Add(allowed);
                    }
                }
            }
            catch { }
        }

        /*
         * Name: BanUserFromGroup
         * Description: Disallows a user to access the given group.          
         * Arguments: The id of the user to ban from and the group to ban them from.
         */
        public void BanUserFromGroup(int bannedUserId, int groupId)
        {
            User u = GetUser();
            try
            {
                User banned = db.Users.First(d => d.UserId == bannedUserId);
                Group g = db.Groups.Single(d => d.GroupID == groupId);
                // check if user 'u' is an owner of group g
                if (g.Users.Contains(u))
                {
                    // make sure the banned user is not also an owner of group g
                    // TODO: have some way to ban owners as well. 
                    if (!g.Users.Contains(banned))
                    {
                        // remove allowance to join group
                        if (g.Users1.Contains(banned))
                        {
                            g.Users1.Remove(banned);
                        }
                        // remove banned user's subscription to group
                        if (g.Users2.Contains(banned))
                        {
                            g.Users2.Remove(banned);
                        }
                    }
                }
            }
            catch { }

        }

        /*
         * Name: GiveUserOwnership
         * Description: Makes a user an owner of the given group.
         * Arguments: The id of the user to ownerify and the group they are being
         *            ownerified to.
         */
        public void GiveUserOwnership(int UserId, int GroupId)
        {
            try
            {
                Group g = db.Groups.Single(d => d.GroupID == GroupId);
                User u = GetUser();
                User toOwn = db.Users.Single(d => d.UserId == UserId);
                if (g.Users.Contains(u))
                {
                    if (!g.Users.Contains(toOwn))
                    {
                        g.Users.Add(toOwn);
                    }
                }
            }
            catch { }
        }

        /*
         * Name: ChangeDescription
         * Description: Changes the current user's profile description
         * Arguments: The new description
         */
        public void ChangeDescription(string NewDescription)
        {
            User curruser = GetUser();
            curruser.ProfileDescription = NewDescription;
        }
        /*
         * Name: Add
         * Description: Adds a user to the database.
         * Arguments: The user to add.
         */
        public void Add(User user)
        {
            db.Users.Add(user);
        }

        /*
         * Name: Delete
         * Description: Removes a user from the database.
         * Arguments: The user to remove.
         */
        public void Delete(User user)
        {
            db.Users.Remove(user);
        }

        /*
         * Name: Save
         * Description: Saves changes to the database.
         * Arguments: None.
         */
        public void Save()
        {
            db.SaveChanges();
        }
    }
}