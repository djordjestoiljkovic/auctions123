using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using auctions123.Models;

namespace auctions123.Controllers
{
    [Authorize(Roles = "Noone")]
    public class LastBiddersController : Controller
    {
        private Model1 db = new Model1();
      
        // GET: LastBidders
        public ActionResult Index()
        {
            var lastBidders = db.LastBidders.Include(l => l.AspNetUser).Include(l => l.Auction);
            return View(lastBidders.ToList());
        }

        // GET: LastBidders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LastBidder lastBidder = db.LastBidders.Find(id);
            if (lastBidder == null)
            {
                return HttpNotFound();
            }
            return View(lastBidder);
        }

        // GET: LastBidders/Create
        public ActionResult Create()
        {
            ViewBag.IdUser = new SelectList(db.AspNetUsers, "Id", "FirstName");
            ViewBag.IdAuction = new SelectList(db.Auctions, "IdAuction", "productName");
            return View();
        }

        // POST: LastBidders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdLastBid,IdAuction,IdUser,Status_")] LastBidder lastBidder)
        {
            if (ModelState.IsValid)
            {
                db.LastBidders.Add(lastBidder);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdUser = new SelectList(db.AspNetUsers, "Id", "FirstName", lastBidder.IdUser);
            ViewBag.IdAuction = new SelectList(db.Auctions, "IdAuction", "productName", lastBidder.IdAuction);
            return View(lastBidder);
        }

        // GET: LastBidders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LastBidder lastBidder = db.LastBidders.Find(id);
            if (lastBidder == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdUser = new SelectList(db.AspNetUsers, "Id", "FirstName", lastBidder.IdUser);
            ViewBag.IdAuction = new SelectList(db.Auctions, "IdAuction", "productName", lastBidder.IdAuction);
            return View(lastBidder);
        }

        // POST: LastBidders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdLastBid,IdAuction,IdUser,Status_")] LastBidder lastBidder)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lastBidder).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdUser = new SelectList(db.AspNetUsers, "Id", "FirstName", lastBidder.IdUser);
            ViewBag.IdAuction = new SelectList(db.Auctions, "IdAuction", "productName", lastBidder.IdAuction);
            return View(lastBidder);
        }

        // GET: LastBidders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LastBidder lastBidder = db.LastBidders.Find(id);
            if (lastBidder == null)
            {
                return HttpNotFound();
            }
            return View(lastBidder);
        }

        // POST: LastBidders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LastBidder lastBidder = db.LastBidders.Find(id);
            db.LastBidders.Remove(lastBidder);
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
