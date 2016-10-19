using auctions123.Models;
using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace auctions123.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private Model1 db = new Model1();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [AllowAnonymous]
        public ActionResult Index(int? page, string searchMin = "", string searchMax = "", string searchName = "", string stages = "none")
        {

            logger.Info("Home/Index/ ");
            if (User.IsInRole("Admin")) {
                return RedirectToAction("Index", "Administrator");
            }
            bool noSearchInputs = false;
           

            ViewBag.stages = stages;
            ViewBag.searchMin = searchMin;
            ViewBag.searchMax = searchMax;
            ViewBag.searchName = searchName;

            if (String.IsNullOrEmpty(searchName)  && String.IsNullOrEmpty(searchMin) && String.IsNullOrEmpty(searchMax) && stages == "none")
            {
                stages = "open";
                noSearchInputs = true;
            }

            var auctions = from o in db.Auctions where o.stage != "DELETED" select o;

            if (stages.Equals("open"))
                auctions = auctions.Where(a => a.stage == "OPEN");
            else if (stages.Equals("ready"))
                auctions = auctions.Where(a => a.stage == "READY");
            else if (stages.Equals("sold"))
                auctions = auctions.Where(a => a.stage == "SOLD");
            else if (stages.Equals("expired"))
            {
                auctions = auctions.Where(a => a.stage == "EXPIRED");
            }

           
            
            if (!String.IsNullOrEmpty(searchName)) {
                string[] searchNames = searchName.Split(null);
             
                auctions = (from a in auctions
                            where searchNames.All(word => a.productName.ToLower().Contains(word.ToLower()))
                            select a);//OVO RADI!
            
            }
        
            if (!String.IsNullOrEmpty(searchMin) || !String.IsNullOrEmpty(searchMax))
            {
                long auctionMinPrice = 0;
                long auctionMaxPrice = long.MaxValue;
                if (!String.IsNullOrEmpty(searchMin))
                {
                    auctionMinPrice = Convert.ToInt64(searchMin);
                    ViewBag.searchMin = auctionMinPrice;
                }
                if (!String.IsNullOrEmpty(searchMax))
                {
                    auctionMaxPrice = Convert.ToInt64(searchMax);
                    ViewBag.searchMax = auctionMaxPrice;
                }
                auctions = auctions.Where(a => (a.price + a.priceChange) >= auctionMinPrice && (a.price + a.priceChange) <= auctionMaxPrice);
            }

        
            auctions = auctions.OrderByDescending(s => s.dateOpened);
            if (noSearchInputs == true) {
                auctions = auctions.Take(5);
            }

            int pageSize = 8;
            int pageNumber = (page ?? 1);
            return View(auctions.ToPagedList(pageNumber, pageSize));
        }

        
        public ActionResult Won(int? page)
        {
            logger.Info("Home/Won/ ");
            AspNetUser user = db.AspNetUsers.Find(User.Identity.GetUserId());
            var  auctions = from a in db.Auctions
                       join l in db.LastBidders on a.IdAuction equals l.IdAuction
                       where l.Status_ == "SOLD" && l.IdUser == user.Id && a.stage != "DELETED"
                       select a;
            //var auctions = from o in db.Auctions select o;
            auctions = auctions.OrderByDescending(s => s.dateOpened);
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            return View(auctions.ToPagedList(pageNumber, pageSize));
        }




        public ActionResult MyOrders(int? page)
        {
            logger.Info("Home/MyOrders/ ");
            AspNetUser user = db.AspNetUsers.Find(User.Identity.GetUserId());
            var orders = from a in db.Orders
                           where a.IdUser == user.Id
                           select a;
            //var auctions = from o in db.Auctions select o;
            orders = orders.OrderByDescending(s => s.dateCreated);
            int pageSize = 16;
            int pageNumber = (page ?? 1);
            return View(orders.ToPagedList(pageNumber, pageSize));
        }

    }
}