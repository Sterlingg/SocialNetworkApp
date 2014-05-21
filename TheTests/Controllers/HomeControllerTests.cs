using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialNetworkApp.Controllers;

using TheTests.Mock;
using Moq;
using SocialNetworkApp.Models;
using System.Web.Mvc;

namespace TheTests.Controllers
{
    [TestClass]
    public class HomeControllerTests
    {

        private HomeController Controller { get; set; }
        private MockSocialContext SocialContext { get; set; }
        private MockUserRepository UserRepository { get; set; }
        private Mock<IWebSecurity> WebSecurity { get; set; }
        private MockDatabase db { get; set; }


        public HomeControllerTests()
        {
            WebSecurity = new Mock<IWebSecurity>(MockBehavior.Strict);
            SetUpContext();
            //Setup the fake security, repos, and db.
            UserRepository = new MockUserRepository(SocialContext, WebSecurity.Object, new PostRepository(SocialContext)
                                                    , new GroupRepository(SocialContext));
            Controller = new HomeController(UserRepository);
        }

        public void SetUpContext()
        {

            db = new MockDatabase();
            SocialContext = new MockSocialContext
            {
                Users = { db.user0, db.user1, db.user2, db.user3, db.user4 },
                Groups = { db.group0, db.group1, db.group2 },
                Locations = { db.location0, db.location1, db.location2 },
                Posts = { db.post0, db.post1, db.post2, db.post3, db.post4 }
            };
        }

        [TestMethod]
        public void TestIndex()
        {
            WebSecurity.Setup(w => w.CurrentUserId()).Returns(1);
            var result = Controller.Index() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ViewName);
        }

    }
}
