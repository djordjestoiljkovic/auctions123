using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using auctions123.Models;
using Microsoft.AspNet.Identity.Owin;

namespace auctions123.Controllers
{
    public class AspNetUsersController : Controller
    {
        private Model1 db = new Model1();

        // GET: AspNetUsers
        [Authorize(Roles = "Noone")]
        public ActionResult Index()
        {
            return View(db.AspNetUsers.ToList());
        }

        // GET: AspNetUsers/Details/5
        [Authorize(Roles = "Noone")]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // GET: AspNetUsers/Create
        [Authorize(Roles = "Noone")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: AspNetUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Noone")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,numOfTokens,Email,PasswordHash,SecurityStamp,UserName")] AspNetUser aspNetUser)
        {
            if (ModelState.IsValid)
            {
                db.AspNetUsers.Add(aspNetUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aspNetUser);
        }

        // GET: AspNetUsers/Edit/5
        [Authorize(Roles = "Admin, Logged")]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // POST: AspNetUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Logged")]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,Email")] EditUserViewModel aspNetUserEdit)
        {
            var aspNetUser = db.AspNetUsers.Find(aspNetUserEdit.Id);
         

            if (ModelState.IsValid)
            {
                var emailChanged = false;

                if (aspNetUser.Email != aspNetUserEdit.Email)
                    emailChanged = true;

                aspNetUser.Email = aspNetUserEdit.Email;
                aspNetUser.UserName = aspNetUserEdit.Email;
                aspNetUser.FirstName = aspNetUserEdit.FirstName;
                aspNetUser.LastName = aspNetUserEdit.LastName;
                db.Entry(aspNetUser).State = EntityState.Modified;
                db.SaveChanges();
                if (emailChanged)
                    return Redirect("http://auctionsiep.azurewebsites.net/");
                return RedirectToAction("Index", "Manage");
            }
            return View(aspNetUser);
        }

        // GET: AspNetUsers/Delete/5
        [Authorize(Roles = "Noone")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // POST: AspNetUsers/Delete/5
        [Authorize(Roles = "Noone")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            db.AspNetUsers.Remove(aspNetUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
