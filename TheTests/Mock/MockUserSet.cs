using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocialNetworkApp.Models;

// Taken from http://romiller.com/2012/02/14/testing-with-a-fake-dbcontext//

namespace TheTests
{
    public class MockUserSet : MockDbSet<User>
    {
        public override User Find(params object[] keyValues)
        {
            try
            {
                return this.SingleOrDefault(u => u.UserId == (int)keyValues.Single());
            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }

    }
}