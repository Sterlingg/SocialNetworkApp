using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocialNetworkApp.Models;

// Taken from http://romiller.com/2012/02/14/testing-with-a-fake-dbcontext//
namespace TheTests
{
    public class MockPostSet : MockDbSet<Post>
    {
        public override Post Find(params object[] keyValues)
        {
            try
            {
                return this.SingleOrDefault(p => p.PostID == (int)keyValues.Single());
            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }
    }
}