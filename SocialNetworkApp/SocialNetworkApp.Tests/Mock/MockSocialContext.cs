using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using SocialNetworkApp.Models;
using RealTests.MockDb;

namespace RealTests
{

    // Taken from http://romiller.com/2012/02/14/testing-with-a-fake-dbcontext//
    // Used for faking a database.

    public class MockSocialContext : ISocialContext
    {
        public IDbSet<Post> Posts { get; set; }
        public IDbSet<Location> Locations { get; set; }
        public IDbSet<Group> Groups { get; set; }
        public IDbSet<User> Users { get; set; }

        public MockSocialContext()
        {
            this.Posts = new MockPostSet();
            this.Locations = new MockLocationSet();
            this.Groups = new MockGroupSet();
            this.Users = new MockUserSet();
        }

        public int SaveChanges()
        {
            return 0;
        }

    }
}
