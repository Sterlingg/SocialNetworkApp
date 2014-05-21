// ***********************************************************************
// Assembly         : SocialNetworkApp
// Author           : Team Valdes
// Created          : 11-16-2012
//
// Last Modified By : Team Valdes
// Last Modified On : 11-16-2012
// ***********************************************************************
// <copyright file="GroupRepository.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNetworkApp.Models
{
    public class GroupRepository : SocialNetworkApp.Models.IGroupRepository
    {

      private ISocialContext db;
        // Not used according to static analysis
      //private IUserRepository ur;

        public GroupRepository()
            : this(ContextHelper.GetContext())
        { }

        public GroupRepository(ISocialContext sc){
            db = sc;
        }

        public IQueryable<Group> GetPopularGroups()
        {
            var ordered = (from g in db.Groups
                           where g.Posts.Count > 0
                           where g.IsPrivate == false
                           orderby g.Posts.Count descending
                           select g);
            if (ordered.Count() > 5)
            {
                return ordered.Take(5);
            }
            return ordered;
        }

        /*
         * Name: NotPrivateOrOwner
         * Description: Takes groups which are not private unioned with private groups of which the user is an owner.
         * Argument: The currently logged in user.
         */
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        //Done as a work around.
        public List<Group> NotPrivateOrOwner(User u)
        {
            var NotPrivate = (from g in db.Groups
                                   where (g.IsPrivate == false)
                                   select g);


            var Private = (from g2 in u.Groups
                           where (g2.IsPrivate == true)
                           select g2);


            List<Group> TestNotPrivate = new List<Group>();
            TestNotPrivate = NotPrivate.ToList();

            List<Group> TestPrivate  = new List<Group>();

            //This if is here to keep user 0, our test user, from hitting this next line.
            if (u.UserId != 0)
            {
                TestPrivate = Private.ToList();
            }

            List<Group> Union = new List<Group>();
            Union.AddRange(TestNotPrivate);
            Union.AddRange(TestPrivate);


            //It is this operation that causes this not to be able to be converted into 
            //a list
            //List<Group> TestUnion = Union.ToList();

            return (Union);

        }

        public IQueryable<Group> SearchOnTerm(string term)
        {
            var groups = from g in
                             (from g in db.Groups
                              where g.IsPrivate == false
                              where g.GroupName.Contains(term)
                              select g).Union(from g in db.Groups
                                              where g.IsPrivate == false
                                              where g.GroupDescription.Contains(term)
                                              select g)
                         orderby g.Posts.Count() descending
                         select g;
            if (groups.Count() > 10)
            {
                return groups.Take(10);
            }
            return groups;
        }

        public void MakePublic(int GroupId)
        {
            try
            {
                Group g = db.Groups.First(d => d.GroupID == GroupId);
                g.IsPrivate = false;
                db.SaveChanges();
            }
            catch
            {
                return;
            }
        }
    }
}