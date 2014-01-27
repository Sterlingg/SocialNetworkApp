using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocialNetworkApp.Models;

namespace TheTests
{
    public class MockDatabase
    {
        // Users
        public User user0 { get; set; }
        public User user1 { get; set; }
        public User user2 { get; set; }
        public User user3 { get; set; }
        public User user4 { get; set; }

        //Groups a user owns.
        public List<Group> ownlist0 { get; set; }

        // Groups
        // Groups.Users2 ie: Subscriptions
        public List<User> sublist0 { get; set; }
        public List<User> sublist1 { get; set; }
        public List<User> sublist2 { get; set; }

        public Group group0 { get; set; }
        public Group group1 { get; set; }
        public Group group2 { get; set; }
        public Group group3 { get; set; }

        //Locations
        public Location location0 { get; set; } // "Main post" users location.
        public Location location1 { get; set; } // Post 1000 meters away from main post.
        public Location location2 { get; set; } // Post >2000 meters away from main post.

        //Posts
        public Post post0 { get; set; }   //Main
        public Post post1 { get; set; }   //Main
        public Post post2 { get; set; }   // Close enough
        public Post post3 { get; set; }   // Too Far. 
        public Post post4 { get; set; }   // Too far.

        public List<Group> subslist { get; set; }

        public MockDatabase()
        {

            SetUpUsersAndGroups();

            SetUpLocations();
            SetUpPosts();

            subslist = new List<Group>();
            subslist.Add(group0);
        }

        //Users

        //Setup the users.
        private void SetUpUsersAndGroups()
        {

            user2 = new User { UserId = 2, UserProfileId = 2 };
            user3 = new User { UserId = 3, UserProfileId = 3 };

            sublist1 = new List<User>();

            sublist1.Add(user2);
            sublist1.Add(user3);

            group1 = new Group { GroupID = 1, Users2 = sublist1, IsPrivate = true }; // Owner
            ownlist0 = new List<Group>();
            ownlist0.Add(group1);

            user0 = new User { UserId = 0, UserProfileId = 0, Groups = ownlist0 };
            user1 = new User { UserId = 1, UserProfileId = 1 };

            sublist0 = new List<User>();

            sublist0.Add(user0);
            sublist0.Add(user1);

            group0 = new Group { GroupID = 0, Users2 = sublist0, IsPrivate = false }; // Private



            sublist2 = new List<User>();

            sublist2.Add(user0);
            sublist2.Add(user3);

            user4 = new User { UserId = 4, UserProfileId = 4 };

            //Set up which group each user is subscribed to.


            group2 = new Group { GroupID = 2, Users2 = sublist2, IsPrivate = false }; // Don't show

            group3 = new Group { GroupID = 3, IsPrivate = true };
        }
        //Groups

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
            post0 = new Post { PostID = 0, LocationID = 0, GroupID = 0, PostTitle = "See", VisibleProximity = 2000 };
            post1 = new Post { PostID = 1, LocationID = 0, GroupID = 1, PostTitle = "NotSee", VisibleProximity = 100 }; //Differentiating groups
            post2 = new Post { PostID = 2, LocationID = 1, GroupID = 2, PostTitle = "See", VisibleProximity = 3000 };
            post3 = new Post { PostID = 3, LocationID = 2, GroupID = 2, PostTitle = "NotSee", VisibleProximity = 2000 };
            post4 = new Post { PostID = 4, LocationID = 2, GroupID = 2, PostTitle = "See", VisibleProximity = 20000 };
        }

    }
}
