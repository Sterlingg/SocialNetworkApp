// ***********************************************************************
// Assembly         : SocialNetworkApp
// Author           : Team Valdes
// Created          : 11-16-2012
//
// Last Modified By : Team Valdes
// Last Modified On : 11-16-2012
// ***********************************************************************
// <copyright file="GroupController.cs" company="">
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
    public class GroupController : Controller
    {
        private SocialContext db = new SocialContext();

        //
        // GET: /Group/
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public ActionResult Index()
        {
            return View(db.Groups.ToList());
        }

        //
        // GET: /Group/Details/5
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public ActionResult Details(int id = 0)
        {
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        //
        // GET: /Group/Create
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Group/Create
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public ActionResult Create(Group group)
        {
            if (ModelState.IsValid)
            {
                db.Groups.Add(group);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(group);
        }

        //
        // GET: /Group/Edit/5
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public ActionResult Edit(int id = 0)
        {
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        //
        // POST: /Group/Edit/5
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public ActionResult Edit(Group group)
        {
            if (ModelState.IsValid)
            {
                db.Entry(group).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(group);
        }

        //
        // GET: /Group/Delete/5
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public ActionResult Delete(int id = 0)
        {
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        //
        // POST: /Group/Delete/5
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, User")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Group group = db.Groups.Find(id);
            db.Groups.Remove(group);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}