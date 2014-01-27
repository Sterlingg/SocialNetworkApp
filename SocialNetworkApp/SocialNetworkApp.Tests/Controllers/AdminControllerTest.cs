  using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Security;
    using Moq;
    using SocialNetworkApp;
    using SocialNetworkApp.Controllers;
    using SocialNetworkApp.Models;
    using System.Security.Principal;
    using System.Collections.Generic;
   using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RealTests
{
    [TestClass]
    public class AdminControllerTests
    {
        private AdminController Controller { get; set; }
        private MockSocialContext Context { get; set; }
        private Mock<IWebSecurity> WebSecurity { get; set; }
   
        public AdminControllerTests()
        {
            Context = new MockSocialContext
            {
                Users =
                {
                    new User {UserId = 0},
                    new User {UserId = 1},
                    new User {UserId = 2},
                    new User {UserId = 3}
                }
            };
  
            WebSecurity = new Mock<IWebSecurity>(MockBehavior.Strict);
            Controller = new AdminController(Context, WebSecurity.Object);
        }

      /*  [TestMethod]
        public void TestIndex()
        {

            var result = Controller.Index();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Test_Index()
        {
            var ac = new AdminController();

            var result = ac.Index();

            Assert.IsNotNull(result);
        }*/

    }
}
