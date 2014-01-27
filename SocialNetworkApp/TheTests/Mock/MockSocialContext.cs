using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using SocialNetworkApp.Models;

namespace TheTests
{

    // Taken from http://romiller.com/2012/02/14/testing-with-a-fake-dbcontext//
    // Used for faking a database.

    public class MockSocialContext : ISocialContext
    {
        public IDbSet<Post> Posts { get; set; }
        public IDbSet<Location> Locations { get; set; }
        public IDbSet<Group> Groups { get; set; }
        public IDbSet<User> Users { get; set; }

        public MockDatabase db { get; set; }

        public MockSocialContext()
        {
            this.Posts = new MockPostSet();
            this.Locations = new MockLocationSet();
            this.Groups = new MockGroupSet();
            this.Users = new MockUserSet();

           /* SetUpUsers();
            SetUpGroupUsers2();
            SetUpGroups();
            SetUpLocations();
            SetUpPosts();*/
        }

      /*  private void SetUpUsers()
        {
            db.user0 = new User { UserId = 0, UserProfileId = 0 };
            user1 = new User { UserId = 1, UserProfileId = 1 };
            user2 = new User { UserId = 2, UserProfileId = 2 };
            user3 = new User { UserId = 3, UserProfileId = 3 };
            user4 = new User { UserId = 4, UserProfileId = 4 };
        }

        //Groups

        //Subscribed users lists.
        private void SetUpGroupUsers2()
        {
            //Set up which group each user is subscribed to.
            sublist0 = new List<User>();
            sublist1 = new List<User>();
            sublist2 = new List<User>();

            sublist0.Add(user0);
            sublist0.Add(user1);

            sublist1.Add(user2);
            sublist1.Add(user3);

            sublist2.Add(user0);
            sublist2.Add(user3);
        }
        //Set up the groups.
        private void SetUpGroups()
        {

            group0 = new Group { GroupID = 0, Users2 = sublist0 };
            group1 = new Group { GroupID = 1, Users2 = sublist1 };
            group2 = new Group { GroupID = 2, Users2 = sublist2 };
        }

        //Set up the locations.
        private void SetUpLocations()
        {
            location0 = new Location { LocationID = 0, Latitude = (Decimal)49.246458, Longitude = (Decimal)(-123.09391) };
            location1 = new Location { LocationID = 1, Latitude = (Decimal)49.243036, Longitude = (Decimal)(-123.088074) };
            location2 = new Location { LocationID = 2, Latitude = (Decimal)49.241078, Longitude = (Decimal)(-122.923622) };
        }

        //Set up the posts.
        private void SetUpPosts()
        {
            post0 = new Post { PostID = 0, LocationID = 0, VisibleProximity = 2000 };
            post1 = new Post { PostID = 1, LocationID = 0, VisibleProximity = 100 };
            post2 = new Post { PostID = 2, LocationID = 1, VisibleProximity = 3000 };
            post3 = new Post { PostID = 2, LocationID = 2, VisibleProximity = 2000 };
            post4 = new Post { PostID = 2, LocationID = 2, VisibleProximity = 2000 };
        }*/
        public int SaveChanges()
        {
            return 0;
        }

    }
}
