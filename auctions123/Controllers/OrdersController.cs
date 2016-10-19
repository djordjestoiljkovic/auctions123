using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using auctions123.Models;
using System.Text;
using Microsoft.AspNet.Identity;
using System.Web.Security;

namespace auctions123.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {

        private Model1 db = new Model1();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Orders
        [Authorize(Roles = "Noone")]
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.AspNetUser);
            return View(orders.ToList());
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
      /*  public ActionResult Create()
        {
            ViewBag.IdUser = new SelectList(db.AspNetUsers, "Id", "FirstName");
            return View();
        }*/

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdOrder,NumOfTokens,IdUser,Price,Status,dateCreated")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdUser = new SelectList(db.AspNetUsers, "Id", "FirstName", order.IdUser);
            return View(order);
        }

  
        public ActionResult CreateOrder()
        {
            Order NewOrder = new Order();
            NewOrder.dateCreated = DateTime.Now;
            var CurrentUser = db.AspNetUsers.Find(User.Identity.GetUserId());
            NewOrder.AspNetUser = CurrentUser;
            NewOrder.Status = "waiting";

            db.Orders.Add(NewOrder);
            db.SaveChanges();

            String crypted = Convert.ToString(NewOrder.IdOrder);
            crypted = crypted + "54986";

            var nonCrypted = Encoding.Unicode.GetBytes(crypted);
            nonCrypted = MachineKey.Protect(nonCrypted,"centili");

            crypted = HttpServerUtility.UrlTokenEncode(nonCrypted);


            var CentiliLink = "https://stage.centili.com/widget/WidgetModule?api=288fd100f846129de9199914003d71ed" + "&clientID=" + crypted;
            return Redirect(CentiliLink);
        }

        [AllowAnonymous]
        public HttpStatusCodeResult UpdateOrder(string clientId, string status, long amount, double enduserprice)
        {

            var crypted = HttpServerUtility.UrlTokenDecode(clientId);
            crypted = MachineKey.Unprotect(crypted, "centili");

            clientId = Encoding.Unicode.GetString(crypted);
            
            if (clientId.Length < 5 || clientId.Substring(clientId.Length - 5, 5) != "54986")
            {
                return new HttpStatusCodeResult(400);
            }

            long IDOrd = Convert.ToInt64(clientId.Substring(0, clientId.Length - 5));

          
            if (status.Equals("failed"))
            {
                Order order = db.Orders.Find(IDOrd);
                order.Status = "failed";
                order.NumOfTokens = (int)amount;
                order.Price = enduserprice;
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
            }
            else 
            { 

                Order order = db.Orders.Find(IDOrd);
                AspNetUser CurrentUser = order.AspNetUser;
                CurrentUser.numOfTokens += amount;
                order.Status = "successful";
                order.NumOfTokens = (int)amount;
                order.Price = enduserprice;
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();

                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                String emailAddress = CurrentUser.Email;

                message.From = new System.Net.Mail.MailAddress("auctionsiep@cheerful.com","Auctions");


                message.To.Add(new System.Net.Mail.MailAddress(emailAddress));
                message.IsBodyHtml = true;
                message.BodyEncoding = Encoding.UTF8;
                message.Subject = "Auctions: Token order";
                message.Body = DateTime.Now + " you have bought " + amount + " tokens " + ", for a price of  " + enduserprice + " dinars.";
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient("smtp.mail.com",587);
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("auctionsiep@cheerful.com", "mailmail123");


          
                client.Send(message);
            }

            return new HttpStatusCodeResult(200);
        }




        // GET: Orders/Edit/5
        [Authorize(Roles = "Noone")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdUser = new SelectList(db.AspNetUsers, "Id", "FirstName", order.IdUser);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Noone")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdOrder,NumOfTokens,IdUser,Price,Status,dateCreated")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdUser = new SelectList(db.AspNetUsers, "Id", "FirstName", order.IdUser);
            return View(order);
        }

        // GET: Orders/Delete/5.
        [Authorize(Roles = "Noone")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [Authorize(Roles = "Noone")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
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
