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
    public class AuctionsController : Controller
    {
        private Model1 db = new Model1();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Auctions
        [Authorize(Roles = "Noone")]
        public ActionResult Index()
        {
            return View(db.Auctions.ToList());
        }

        // GET: Auctions/Details/5
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            logger.Info("Auctions/Details/ ");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auction auction = db.Auctions.Find(id );
            if (auction == null)
            {
                return HttpNotFound();
            }
            var bids = (from b in db.Bids
                       where b.IdAuction == auction.IdAuction
                       select b).OrderByDescending(b=> b.Date).Take(10);
            ViewBag.bids = bids.ToList();
            return View(auction);
        }

        // GET: Auctions/Create
        [Authorize(Roles = "Noone")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Auctions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Noone")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "productName,duration,dateCreated,dateOpened,dateClosed,stage,image,price")] Auction auction)
        {
            if (ModelState.IsValid)
            {
                db.Auctions.Add(auction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(auction);
        }

        // GET: Auctions/Edit/5
        [Authorize(Roles = "Noone")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auction auction = db.Auctions.Find(id);
            if (auction == null)
            {
                return HttpNotFound();
            }
            return View(auction);
        }

        // POST: Auctions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Noone")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdAuction,productName,duration,dateCreated,dateOpened,dateClosed,stage,image,price")] Auction auction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(auction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(auction);
        }

        // GET: Auctions/Delete/5
        [Authorize(Roles = "Noone")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auction auction = db.Auctions.Find(id);
            if (auction == null)
            {
                return HttpNotFound();
            }
            return View(auction);
        }

        // POST: Auctions/Delete/5
        [Authorize(Roles = "Noone")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Auction auction = db.Auctions.Find(id);
            db.Auctions.Remove(auction);
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
