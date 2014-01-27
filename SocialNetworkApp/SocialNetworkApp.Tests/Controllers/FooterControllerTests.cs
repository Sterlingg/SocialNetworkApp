using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Moq;
using SocialNetworkApp;
using SocialNetworkApp.Controllers;
using SocialNetworkApp.Models;
using System.Data;
using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RealTests.MockDb;

namespace RealTests
{
    [TestClass]
    public class FooterControllerTests
    {
        private FooterController Controller { get; set; }
        private MockSocialContext Context { get; set; }
        private Mock<IWebSecurity> WebSecurity { get; set; }

        private User user0, user1, user2, user3;
        private Group group0, group1;

        public FooterControllerTests()
        {

            //Setup the fake security.
            WebSecurity = new Mock<IWebSecurity>(MockBehavior.Strict);


            //Setup the database.
            user0 = new User { UserId = 0, UserProfileId = 0 };
            user1 = new User { UserId = 1, UserProfileId = 1 };
            user2 = new User { UserId = 3, UserProfileId = 3 };
            user3 = new User { UserId = 4, UserProfileId = 4 };

            group0 = new Group { GroupID = 0, Users2 = { user0, user1 } };
            group1 = new Group { GroupID = 1, Users2 = { user2, user3 } };

            Context = new MockSocialContext
            {
                Users = { user0, user1, user2, user3 },
                Groups = { group0, group1 }
            };

            Controller = new FooterController(new UserRepository(Context, WebSecurity.Object));
        }

        [TestMethod]
        public void Test_Subscription()
        {
            WebSecurity.Setup(w => w.CurrentUserId()).Returns(1);
            var result = Controller._Subscriptions() as ViewResult;
            Assert.IsInstanceOfType(result.ViewData.Model, typeof(IEnumerable<Group>));
        }
    }
}
