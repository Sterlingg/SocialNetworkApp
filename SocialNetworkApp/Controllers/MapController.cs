// ***********************************************************************
// Assembly         : SocialNetworkApp
// Author           : Team Valdes
// Created          : 11-16-2012
//
// Last Modified By : Team Valdes
// Last Modified On : 11-17-2012
// ***********************************************************************
// <copyright file="MapController.cs" company="">
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
using System.Data.Objects;

namespace SocialNetworkApp.Controllers
{
    public class MapController : Controller
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private ISocialContext db;
        private IUserRepository ur;

        /*
         *  Name: MapController
         *  Description: Set up the controller for web deployment with a
         *               concrete SocialContext, and UserRepository.
         */
        public MapController()
            : this(ContextHelper.GetContext(), new UserRepository()) { }

        /*
         *  Name: MapController
         *  Description: Constructer that allows the UserRepository, and SocialContext to be injected.                   
         */
        public MapController(ISocialContext sc, IUserRepository userrepo)
        {
            db = sc;
            ur = userrepo;
        }

        /*  
         *  Name: GetPosts
         *  Description: Returns posts filtered by what the user is subscribed to
         *               and the posts within range of their current location.
         *  Arguments: The user's latitude and the users longitude.
         *  View: None.
         */
        [HttpGet]
        public JsonResult GetPosts(string lat, string lon)
        {

            User CurrUser = ur.GetUser();

            if (CurrUser != null && lat != null && lon != null)
                return Json(ur.GetPostsLoggedIn(lat, lon), JsonRequestBehavior.AllowGet);
            else
                return Json(ur.GetPostsNotLoggedIn(lat, lon), JsonRequestBehavior.AllowGet);
        }
    }
}
