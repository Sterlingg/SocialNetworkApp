// ***********************************************************************
// Assembly         : SocialNetworkApp
// Author           : Team Valdes
// Created          : 11-16-2012
//
// Last Modified By : Team Valdes
// Last Modified On : 11-16-2012
// ***********************************************************************
// <copyright file="PostRepository.cs" company="">
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
    public class PostRepository : SocialNetworkApp.Models.IPostRepository
    {
        private ISocialContext db;

        public PostRepository()
            : this(ContextHelper.GetContext())
        {
        }

        public PostRepository(ISocialContext sc)
        {
            db = sc;
        }


        /**
         * Name: FindNearbyPosts
         * Description: Finds posts based on the user's location and the posts
         *              visible proximity by checking if the user is within the posts radius.
         * Arguments: The user's current location in latitude and longitude.
         */
        public IQueryable<Post> FindNearbyPosts(decimal lat, decimal lon)
        {
            // Note: only works in Vancouver for now. See DistanceisGood for (vast) improvement using
            // great circle distance.
            IQueryable<Location> locations = db.Locations;

                   return from p in db.Posts
                   from l in locations
                   where ((p.VisibleProximity.HasValue &&
                   ((((l.Latitude - lat) * (l.Latitude - lat) * ((decimal)(111212.22)) * ((decimal)(111212.22))) +
                   ((l.Longitude - lon) * (l.Longitude - lon) * ((decimal)(111212.22)) * ((decimal)(111212.22)))) < (p.VisibleProximity * p.VisibleProximity))
                   && (p.LocationID == l.LocationID))
                   || ((!p.VisibleProximity.HasValue) && (p.LocationID == l.LocationID))             
                        ) 
                   select p;
        }

        //Not used, but should be used.
        public bool DistanceIsGood(int postId, double lat, double lon, decimal distance)
        {
            decimal earthsradius = 6371009;

            Post p = db.Posts.SingleOrDefault(d => d.PostID == postId);
            Location l = p.Location;
            double lat2 = (double)l.Latitude;
            double lon2 = (double)l.Longitude;
            //Not used according to static analysis
            //double hDist = lat - lat2;
            //double vDist = lon - lon2;
            double realDistance = ((double)earthsradius * (Math.Acos(
                   (Math.Sin((double)lat2) * Math.Sin((double)lat)
                   + Math.Cos((double)lat2) * Math.Cos((double)lat)) * Math.Cos((double)(lon - lon2))
                   ))
                   );
            double maxDistance = (double)distance;
            if (realDistance <= maxDistance)
                return true;
            return false;
        }


        public IQueryable<Post> FindAllPosts()
        {
            return db.Posts;
        }

        public IQueryable<Post> FindPostByGroup(int groupId)
        {
            return db.Posts.Where(d => d.GroupID == groupId);
        }

        public Post GetPost(int id)
        {
            return db.Posts.SingleOrDefault(d => d.PostID == id);
        }

        public void Add(Post post)
        {
            db.Posts.Add(post);
        }

        public void Delete(Post post)
        {
            //Perhaps need to do the SQL to remove the blocks that users have on a post, not sure.
            db.Posts.Remove(post);
        }

        public void Save()
        {
            db.SaveChanges();
        }

    }
}