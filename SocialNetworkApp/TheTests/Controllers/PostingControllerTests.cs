using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SocialNetworkApp.Controllers;
using SocialNetworkApp.Models;
using Moq;
using TheTests.Mock;
using System.Web.Mvc;

namespace TheTests.Controllers
{
    [TestClass]
    public class PostingControllerTests
    {

        private PostingController Controller { get; set; }
        private MockSocialContext SocialContext { get; set; }
        private MockUserRepository UserRepository { get; set; }
        private MockDatabase db { get; set; }
        private Mock<IWebSecurity> WebSecurity { get; set; }

        public PostingControllerTests()
        {
            WebSecurity = new Mock<IWebSecurity>(MockBehavior.Strict);

            SetUpContext();

            //Setup the fake security, repos, and db.
            UserRepository = new MockUserRepository(SocialContext, WebSecurity.Object, new PostRepository(SocialContext)
                                                    , new GroupRepository(SocialContext));

            Controller = new PostingController(SocialContext, UserRepository, new GroupRepository(SocialContext));
        }

        public void SetUpContext()
        {

            db = new MockDatabase();
            SocialContext = new MockSocialContext
            {
                Users = { db.user0, db.user1, db.user2, db.user3, db.user4 },
                Groups = { db.group0, db.group1, db.group2, db.group3 },
                Locations = { db.location0, db.location1, db.location2 },
                Posts = { db.post0, db.post1, db.post2, db.post3, db.post4 }
            };
        }

        [TestMethod]
        public void Test_MakePost()
        {
            WebSecurity.Setup(w => w.CurrentUserId()).Returns(0);

            var result = Controller._MakePost() as PartialViewResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ViewBag.GroupList);

            foreach (Group g in result.ViewBag.TestingGroupList)
            {
                if (g.IsPrivate == true)
                {
                    //If a private group make sure they are an owner.
                    Assert.IsTrue(db.user0.Groups.Contains(g));
                }
                //Private group not an owner of.
                Assert.IsFalse(g.GroupID == 3);
            }

            Assert.AreEqual("_MakePost", result.ViewName);
        }

        [TestMethod]
        public void Test_MakePost1ArgValid()
        {
            WebSecurity.Setup(w => w.CurrentUserId()).Returns(0);

            Post APost = new Post { UserID = 988 };
            Location ALocation = new Location { LocationID = 988 };
            PostLocationModel APlm = new PostLocationModel();
            APlm.Tpost = APost;
            APlm.Tlocation = ALocation;

            var result = Controller._MakePost(APlm) as JavaScriptResult;
                
            Assert.IsNotNull(result);
            Assert.IsTrue(SocialContext.Posts.Contains(APost));
            Assert.IsTrue(SocialContext.Locations.Contains(ALocation));
            Assert.AreEqual(result.Script, "$('#postcontainer').hide(); removeRadiusSelctor();"
                                       + "updateMarkers();");

        }
        [TestMethod]
        public void Test_MakePost1ArgInValid()
        {
            Controller.ModelState.AddModelError("key", "Error.");
            WebSecurity.Setup(w => w.CurrentUserId()).Returns(0);

            Post APost = new Post { UserID = 988 };
            Location ALocation = new Location { LocationID = 988 };
            PostLocationModel APlm = new PostLocationModel();
            APlm.Tpost = APost;
            APlm.Tlocation = ALocation;

            var result = Controller._MakePost(APlm) as PartialViewResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ViewBag.GroupList);

            foreach (Group g in result.ViewBag.TestingGroupList)
            {
                if (g.IsPrivate == true)
                {
                    //If a private group make sure they are an owner.
                    Assert.IsTrue(db.user0.Groups.Contains(g));
                }
                //Private group not an owner of.
                Assert.IsFalse(g.GroupID == 3);
            }

            Assert.AreEqual("_MakePost", result.ViewName);
        }

    }


}
