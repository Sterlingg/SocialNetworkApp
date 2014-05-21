// ***********************************************************************
// Assembly         : SocialNetworkApp
// Author           : Team Valdes
// Created          : 11-16-2012
//
// Last Modified By : Team Valdes
// Last Modified On : 11-17-2012
// ***********************************************************************
// <copyright file="AdminController.cs" company="">
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
    public class AdminController : Controller
    {
        private ISocialContext db;
           private IWebSecurity WebSecurity { get; set; }

          public AdminController()
            : this(ContextHelper.GetContext(), new WebSecurityWrapper())
        { }
        public AdminController(ISocialContext sc, IWebSecurity webSecurity)
        {
            WebSecurity = webSecurity;
            db = sc;
        }


        //
        // GET: /Admin/
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Index()
        {
            return View("Index",db.Users.ToList());
        }

        //
        // GET: /Admin/
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult _Index(String uName)
        {
            System.Web.Security.Roles.RemoveUserFromRole(uName, "User");
            System.Web.Security.Roles.AddUserToRole(uName, "Admin");

            return RedirectToAction("Index");
            
        }
    }
}