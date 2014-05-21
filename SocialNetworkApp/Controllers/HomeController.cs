// ***********************************************************************
// Assembly         : SocialNetworkApp
// Author           : Team Valdes
// Created          : 11-16-2012
//
// Last Modified By : Team Valdes
// Last Modified On : 11-17-2012
// ***********************************************************************
// <copyright file="HomeController.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SocialNetworkApp.Models;
using WebMatrix.WebData;
using SocialNetworkApp.Filters;

namespace SocialNetworkApp.Controllers
{

    [InitializeSimpleMembership]
    public class HomeController : Controller
    {

         private IUserRepository ur;

        /*
         *  Name: HomeController
         *  Description: Set up the controller for web deployment with a
         *               concrete user repository.
         */
         public HomeController()
             : this(new UserRepository())
         { }

        /*
         *  Name: HomeController
         *  Description: Constructer that allows the UserRepository to be injected.                   
         */
        public HomeController(IUserRepository userrepo)
        {
            ur = userrepo;
        }

        /*  
         *  Name: Index 
         *  Description: Returns the main map page.
         *  View: /Home/Index/
         */ 
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Message = "Here we goooooo!";

            return View("Index");
        }

    }
}