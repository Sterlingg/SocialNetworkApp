// ***********************************************************************
// Assembly         : SocialNetworkApp
// Author           : Team Valdes
// Created          : 11-16-2012
//
// Last Modified By : Team Valdes
// Last Modified On : 11-16-2012
// ***********************************************************************
// <copyright file="UserController.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SocialNetworkApp.Models;

namespace SocialNetworkApp.Controllers
{
    public class UserController : Controller
    {
        private SocialContext db = new SocialContext();

        //
        // GET: /User/
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        //
        //GET: /User/Details/
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public ActionResult Details(int id = 0)
        {
            //Our keys in the UserProfile and User table are off by one
            //This is the solution because deleting the contents of the DB didn't help

            UserRepository ur = new UserRepository();
            User user = ur.GetUser(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

    }
}
