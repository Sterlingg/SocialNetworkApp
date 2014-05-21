// ***********************************************************************
// Assembly         : SocialNetworkApp
// Author           : Sterling
// Created          : 11-16-2012
//
// Last Modified By : Sterling
// Last Modified On : 11-16-2012
// ***********************************************************************
// <copyright file="UserRepository.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
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

        private SocialContext db;

        public UserRepository()
        {
            db = ContextHelper.GetContext();
        }

        private class UserComparer : IEqualityComparer<User>
        {
            public bool Equals(User x, User y)
            {
                return (x.UserId == y.UserId);
            }

            // Don't use this...
            public int GetHashCode(User obj)
            {
                return -1;
            }
        }

        public User GetUser()
        {
            //I commented this out because when I brought in the interfaces for WebSecurity and OAuthWebSecurity
            //It claims that it is always true as an int is never == to null.

            //  if (WebSecurity.CurrentUserId != null)
            // {
            return db.Users.FirstOrDefault(d => d.UserProfileId == WebSecurity.CurrentUserId);
            //  }

            // Possible blow up here
            // return null;
            //  throw new UnauthorizedAccessException("No user is currently logged in.");
        }

        public User GetUser(int userProfileId)
        {
            return db.Users.FirstOrDefault(d => d.UserProfileId == userProfileId);
        }

        public IQueryable<Group> GetSubscriptions()
        {
            User CurrUser = GetUser();
            int uid = CurrUser.UserId;

            return (from gr in db.Groups
                    from u in gr.Users2
                    where (u.UserId == uid)
                    select gr);
        }

        public IQueryable<Group> GetOwnedGroups()
        {
            User CurrUser = GetUser();
            int uid = CurrUser.UserId;
            return (from gr in db.Groups
                    from u in gr.Users
                    where (u.UserId == uid)
                    select gr);

        }

        public IQueryable<Group> GetInvitedGroups()
        {
            User CurrUser = GetUser();
            int uid = CurrUser.UserId;
            return from gr in db.Groups
                   from u in gr.Users1
                   where (u.UserId == uid)
                   select gr;

        }

        public bool IsSubscribedTo(int gid)
        {
            var subscriptions = GetSubscriptions().ToList();
            Group g = db.Groups.FirstOrDefault(d => d.GroupID == gid);
            if (subscriptions.Contains(g))
            {
                return true;
            }
            return false;
        }

        public bool IsOwnerOf(int gid)
        {
            var ownerships = GetOwnedGroups().ToList();
            Group g = db.Groups.FirstOrDefault(d => d.GroupID == gid);
            if (ownerships.Contains(g))
            {
                return true;
            }
            return false;
        }

        public IQueryable<User> GetBlockedUsers()
        {
            User u = GetUser();
            return from g in db.Users
                   where g.Users.Contains(u)
                   select g;
        }

        public IQueryable<Post> GetBlockedPosts()
        {
            User u = GetUser();
            return from g in db.Posts
                   where g.Users.Contains(u)
                   select g;
        }

        public IQueryable<Post> GetCreatedPosts()
        {
            User u = GetUser();
            return from g in db.Posts
                   where g.User.Equals(u)
                   select g;
        }


        // adds/removes to many-to-many relationships


        public void AddCreatedGroup(int groupId)
        {
            User u = GetUser();
            Group g = db.Groups.SingleOrDefault(d => d.GroupID == groupId);
            if (!g.Users.Contains(u))
            {
                g.Users.Add(u);
            }
        }

        public void RemoveCreatedGroup(int groupId)
        {
            User u = GetUser();
            Group g = db.Groups.SingleOrDefault(d => d.GroupID == groupId);
            if (g.Users.Contains(u))
            {
                if (g.IsPrivate)
                {
                    // g is not the only owner of a private group
                    if (g.Users.Count() > 1)
                    {
                        g.Users.Remove(u);
                    }
                    else
                    {
                        //TODO: Exception alerting user that if the groups is deleted than everything in the group is deleted
                    }
                }
                else
                {
                    g.Users.Remove(u); //Public Groups can have no creators.
                }
            }
        }


        public void AddSubscription(int groupId)
        {
            User u = GetUser();
            Group g = db.Groups.SingleOrDefault(d => d.GroupID == groupId);
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

        public void RemoveSubscription(int groupId)
        {
            User u = GetUser();
            Group g = db.Groups.SingleOrDefault(d => d.GroupID == groupId);
            if (g.Users2.Contains(u))
                g.Users2.Remove(u);

            // Question: If group is private, and you remove subscription, 
            // should you still be on the "allowed users" list?
            // (currently not implemented)
        }

        public void BlockUser(string blockUserName)
        {
            User u = GetUser();
            User toBlock = db.Users.FirstOrDefault(b => b.UserName == blockUserName);
            if (!u.User1.Contains(toBlock))
                u.User1.Add(toBlock);
        }

        public void UnBlockUser(string unblockUserName)
        {
            User u = GetUser();
            User toUnBlock = db.Users.FirstOrDefault(b => b.UserName == unblockUserName);
            if (u.User1.Contains(toUnBlock))
                u.User1.Remove(toUnBlock);
        }

        public void BlockPost(int blockId)
        {
            User u = GetUser();
            Post toBlock = db.Posts.SingleOrDefault(d => d.PostID == blockId);
            if (!u.Posts1.Contains(toBlock))
                u.Posts1.Add(toBlock);
        }

        public void UnBlockPost(int blockedId)
        {
            User u = GetUser();
            Post toUnBlock = db.Posts.SingleOrDefault(d => d.PostID == blockedId);
            if (u.Posts1.Contains(toUnBlock))
                u.Posts1.Remove(toUnBlock);
        }

        public void AllowUserSubscribeToGroup(int allowedUserId, int groupId)
        {
            User u = GetUser();
            User allowed = db.Users.SingleOrDefault(d => d.UserId == allowedUserId);
            Group g = db.Groups.SingleOrDefault(d => d.GroupID == groupId);

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
        public void BanUserFromGroup(int bannedUserId, int groupId)
        {
            User u = GetUser();
            User banned = db.Users.FirstOrDefault(d => d.UserId == bannedUserId);
            Group g = db.Groups.SingleOrDefault(d => d.GroupID == groupId);
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

        public void GiveUserOwnership(int UserId, int GroupId)
        {
            Group g = db.Groups.SingleOrDefault(d => d.GroupID == GroupId);
            User u = GetUser();
            User toOwn = db.Users.SingleOrDefault(d => d.UserId == UserId);
            if (g.Users.Contains(u))
            {
                if (!g.Users.Contains(toOwn))
                {
                    g.Users.Add(toOwn);
                }
            }
        }

        public void Add(User user)
        {
            db.Users.Add(user);
        }

        public void Delete(User user)
        {
            db.Users.Remove(user);
        }

        public void Save()
        {
            db.SaveChanges();
        }



    }
}