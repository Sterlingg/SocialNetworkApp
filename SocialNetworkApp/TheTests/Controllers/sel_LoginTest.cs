//using System;
//using System.Text;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using OpenQA.Selenium.Firefox;
//using OpenQA.Selenium;

//namespace SocialNetworkApp.Tests.Controllers
//{
//    [TestClass]
//    public class sel_LoginTest
//    {
//        private IWebDriver driver;
//        private StringBuilder verificationErrors;
//        private string baseURL;

//        [TestInitialize]
//        public void SetupTest()
//        {
//            driver = new FirefoxDriver();
//            baseURL = "http://valdes.azurewebsites.net/";
//            verificationErrors = new StringBuilder();
//        }

//        [TestCleanup]
//        public void TeardownTest()
//        {
//            try
//            {
//                driver.Quit();
//            }
//            catch (Exception)
//            {
//                // Ignore errors if unable to close the browser
//            }
//            Assert.AreEqual("", verificationErrors.ToString());
//        }

//        [TestMethod]
//        public void TheLoginTest()
//        {
//            driver.Navigate().GoToUrl(baseURL + "/");
//            driver.FindElement(By.Id("loginLink")).Click();
//        }
//    }
//}
