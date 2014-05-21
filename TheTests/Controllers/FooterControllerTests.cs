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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheTests;

namespace TheTests
{

    [TestClass]
    public class FooterControllerTests
    {
        private FooterController Controller { get; set; }
        private Mock <ISocialContext>SocialContext { get; set; }
        private Mock<IUserRepository> UserRepository { get; set; }
        private Mock<IWebSecurity> WebSecurity { get; set; }
        private MockDatabase db { get; set; }
    
        public FooterControllerTests()
        {
            //Setup the fake security, repos, and db.
            WebSecurity = new Mock<IWebSecurity>(MockBehavior.Strict);
            UserRepository = new Mock<IUserRepository>(MockBehavior.Strict);

            db = new MockDatabase();

            UserRepository.Setup(u => u.GetUser()).Returns(db.user0);
            UserRepository.Setup(u => u.GetSubscriptions()).Returns(db.subslist.AsQueryable());
                
            Controller = new FooterController(UserRepository.Object);          
        }

        [TestMethod]
        public void Test_Subscription()
        {
            WebSecurity.Setup(w => w.CurrentUserId()).Returns(1);
            var result = Controller._Subscriptions() as PartialViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.ViewData.Model, typeof(IEnumerable<Group>));
            Assert.AreEqual("_Subscriptions", result.ViewName);
            var grouplist = result.ViewData.Model as List<Group>;
   
           Assert.AreEqual(db.group0,(grouplist.First()));
        }
    }
}
