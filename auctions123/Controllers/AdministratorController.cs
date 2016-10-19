using auctions123.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace auctions123.Controllers
{
    public class AdministratorController : Controller
    {
        private Model1 db = new Model1();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // GET: Auctions
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            logger.Info("Admin/Index/ ");
            return View(db.Auctions.ToList());
        }

        // GET: Auctions/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int? id)
        {
            logger.Info("Admin/Details/ ");
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

        // GET: Auctions/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Auctions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        //try to bind auctionModel
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AuctionViewModel auctionModel)
        {
            logger.Info("Admin/Create/ ");
            auctionModel.auction.image = new byte[auctionModel.ImageToUpload.ContentLength];
            auctionModel.ImageToUpload.InputStream.Read(auctionModel.auction.image, 0, auctionModel.auction.image.Length);
            auctionModel.auction.dateCreated = DateTime.Now;
            auctionModel.auction.stage = "READY";
            auctionModel.auction.priceChange = 0;
            db.Auctions.Add(auctionModel.auction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Auctions/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            logger.Info("Admin/Edit/id ");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Auction auction = db.Auctions.Find(id);
            if (auction == null || auction.stage != "READY")
            {
                return HttpNotFound();
            }
            return View(auction);
        }

        // POST: Auctions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdAuction,price")]EditViewModel auctionEdit)
        {
            logger.Info("Admin/Edit/id ");
            var auction = db.Auctions.Find(auctionEdit.IdAuction);
            auction.price = auctionEdit.price;
            if (ModelState.IsValid)
            {
                db.Entry(auction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(auction);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Open(int? id)
        {
            logger.Info("Admin/Open/id ");
            if (ModelState.IsValid)
            {
                Auction auction = db.Auctions.Find(id);

                if (auction.stage == "OPEN") return RedirectToAction("Index");

                auction.stage = "OPEN";
                auction.dateOpened = DateTime.Now;
                db.Entry(auction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }


        // GET: Auctions/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            logger.Info("Admin/Delete/id ");
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
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            logger.Info("Admin/DeleteConfirmed/id ");
            Auction auction = db.Auctions.Find(id);
            if (auction.LastBidders.Count > 0)
            {
                var lastBidder = db.LastBidders.SingleOrDefault(a => a.IdAuction == id);
                lastBidder.Status_ = "DELETED";
                db.Entry(lastBidder).State = EntityState.Modified;
            }

            auction.stage = "DELETED";
            db.Entry(auction).State = EntityState.Modified;
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