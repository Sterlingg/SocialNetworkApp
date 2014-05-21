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
using TheTests.Mock;
using System.Web.Script.Serialization;

namespace TheTests.Controllers
{
    //For JSON parsing.
    public class ResultContainerObject{
        public string Latitude {get;set;}
        public string Longitude {get;set;}
        public string Content {get;set;}
        public string Title {get;set;}
        public string GroupID {get;set;}
    }

    [TestClass]
    public class MapControllerTests
    {
        private MapController Controller { get; set; }
        private MockSocialContext SocialContext { get; set; }
        private MockUserRepository UserRepository { get; set; }
        private MockDatabase db { get; set; }
        private Mock<IWebSecurity> WebSecurity { get; set; }

        public MapControllerTests()
        {         
            WebSecurity = new Mock<IWebSecurity>(MockBehavior.Strict);
                      
            SetUpContext();

            //Setup the fake security, repos, and db.
            UserRepository = new MockUserRepository(SocialContext,WebSecurity.Object,new PostRepository(SocialContext)
                                                    , new GroupRepository(SocialContext));

            Controller = new MapController(SocialContext,UserRepository);
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
        public void GetPostsLoggedIn()
        {
            const uint EXPECTED_NUM_POSTS = 3;
            WebSecurity.Setup(w => w.CurrentUserId()).Returns(0);
           
            var result = Controller.GetPosts("49.246458", "-123.09391") as JsonResult;
            Assert.IsNotNull(result);

            //Convert the result to objects.
            string json = (new JavaScriptSerializer().Serialize(result.Data));
            List<ResultContainerObject> rh = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<ResultContainerObject>>(json);

            uint expectedpostscount = 0;
            foreach (var item in rh)
            {
                Assert.IsFalse(item.Title.Equals("NotSee"));
                if (item.Title.Equals("See"))
                {
                    expectedpostscount++;
                }
         
            }
            Assert.IsTrue(expectedpostscount == EXPECTED_NUM_POSTS);
        }

        [TestMethod]
        public void GetPostsNotLoggedIn()
        {
            const int SOME_POST_NOT_IN_DB = 9999;
            const uint EXPECTED_NUM_POSTS = 4;

            WebSecurity.Setup(w => w.CurrentUserId()).Returns(SOME_POST_NOT_IN_DB);
            
            var result = Controller.GetPosts("49.246458", "-123.09391") as JsonResult;
            Assert.IsNotNull(result);

            //Convert the result to objects.
            string json = (new JavaScriptSerializer().Serialize(result.Data));
            List<ResultContainerObject> rh = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<List<ResultContainerObject>>(json);

            uint expectedpostscount = 0;
            foreach (var item in rh)
            {
                    expectedpostscount++;
            }
            Assert.IsTrue(expectedpostscount == EXPECTED_NUM_POSTS);
        }

    }
}
