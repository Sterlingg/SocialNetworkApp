using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocialNetworkApp.Models;

namespace TheTests.Mock
{
    class MockUserRepository : IUserRepository
    {
        private ISocialContext db;
        private IPostRepository pr;
        private IGroupRepository gr;
        private IWebSecurity WebSecurity { get; set; }

        public MockUserRepository(ISocialContext sc, IWebSecurity webSecurity
                              , IPostRepository postrepo, IGroupRepository grouprepo)
        {
            db = sc;
            WebSecurity = webSecurity;
            pr = postrepo;
            gr = grouprepo;
        }

        public User GetUser()
        {
            return db.Users.Find(WebSecurity.CurrentUserId());
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

        public IQueryable GetPostsLoggedIn(string lat, string lon)
        {
            IQueryable<Group> UsersGroups = GetSubscriptions();
            IQueryable<Post> PostsInProximity = pr.FindNearbyPosts(decimal.Parse(lat), decimal.Parse(lon));
            IQueryable<Location> locations = db.Locations;

            return (
                     from g in UsersGroups
                     from l in locations
                     from p in PostsInProximity
                     where (l.LocationID == p.LocationID) && (g.GroupID == p.GroupID) 
                     // && (p.EndDate.HasValue && (p.EndDate < today))
                     select new
                     {
                         Latitude = l.Latitude,
                         Longitude = l.Longitude,
                         Content = p.PostContent,
                         Title = p.PostTitle,
                         GroupID = p.GroupID
                     });
        }

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
                   && (g.IsPrivate == false)   // && (p.EndDate.HasValue && (p.EndDate < today))                     
             select new
             {
                 Latitude = l.Latitude,
                 Longitude = l.Longitude,
                 Content = p.PostContent,
                 Title = p.PostTitle,
                 GroupID = p.GroupID
             });
        }

        public IQueryable<Group> GetOwnedGroups()
        {
            throw new NotImplementedException();
        }

        public IQueryable<Group> GetInvitedGroups()
        {
            throw new NotImplementedException();
        }

        public IQueryable<User> GetBlockedUsers()
        {
            throw new NotImplementedException();
        }

        public IQueryable<Post> GetBlockedPosts()
        {
            throw new NotImplementedException();
        }

        public IQueryable<Post> GetCreatedPosts()
        {
            throw new NotImplementedException();
        }

        public bool IsSubscribedTo(int gid)
        {
            throw new NotImplementedException();
        }

        public bool IsOwnerOf(int gid)
        {
            throw new NotImplementedException();
        }

        public void AddCreatedGroup(int groupId)
        {
            throw new NotImplementedException();
        }

        public void RemoveCreatedGroup(int groupId)
        {
            throw new NotImplementedException();
        }

        public void AddSubscription(int groupId)
        {
            throw new NotImplementedException();
        }

        public void RemoveSubscription(int groupId)
        {
            throw new NotImplementedException();
        }

        public void BlockUser(string blockUserName)
        {
            throw new NotImplementedException();
        }

        public void UnBlockUser(string unblockUserName)
        {
            throw new NotImplementedException();
        }

        public void BlockPost(int blockId)
        {
            throw new NotImplementedException();
        }

        public void UnBlockPost(int blockedId)
        {
            throw new NotImplementedException();
        }

        public void AllowUserSubscribeToGroup(int allowedUserId, int groupId)
        {
            throw new NotImplementedException();
        }

        public void BanUserFromGroup(int bannedUserId, int groupId)
        {
            throw new NotImplementedException();
        }

        public void GiveUserOwnership(int UserId, int GroupId)
        {
            throw new NotImplementedException();
        }

        public void Add(User user)
        {
            throw new NotImplementedException();
        }

        public void Delete(User user)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }
    }
}
