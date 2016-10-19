using System;
using System.Linq;
using Microsoft.AspNet.SignalR;
using System.Data.Entity;
using System.Text;
using System.Net.Mail;
using FluentScheduler;
using auctions123.Models;

namespace auctions123.Hubs
{
    

    public static class mutex
    {
        public static string locky = "MX";
    }

    public class AuctionsHub : Hub
    {
      
        public void Send(int IdAuction)
        {

            String userEmail = Context.User.Identity.Name;
           
                Model1 db = new Model1();
                var auction = db.Auctions.Find(IdAuction);
                var user = db.AspNetUsers.SingleOrDefault(a => a.Email == userEmail);
             lock (mutex.locky)
            {
                
                if (auction.stage != "OPEN") return;
                if (auction.LastBidders.Count == 0)
                {
                    if (user.numOfTokens < (auction.price + (long)auction.priceChange)) { Clients.All.tokenswarning(userEmail); return; }

                    user.numOfTokens -= (auction.price + (long)auction.priceChange);
                    auction.priceChange += 1;
                    
                    
                    LastBidder lastBidder = new LastBidder();
                    lastBidder.AspNetUser = user;
                    lastBidder.Auction = auction;
                    lastBidder.Status_ = auction.stage;
                    db.LastBidders.Add(lastBidder);
                    
                }
                else
                {
                    LastBidder lastBidder = db.LastBidders.SingleOrDefault(a => a.Auction.IdAuction == IdAuction);
                    if (lastBidder.AspNetUser.Email == userEmail)
                    {
                        if (user.numOfTokens == 0) { Clients.All.tokenswarning(userEmail); return; }
                        user.numOfTokens -= 1;
                        auction.priceChange += 1;
                       
                    }
                    else
                    {
                        if (user.numOfTokens < (auction.price + (long)auction.priceChange)) { Clients.All.tokenswarning(userEmail); return; }
                        user.numOfTokens -= (auction.price + (long)auction.priceChange);
                        lastBidder.AspNetUser.numOfTokens += (auction.price + (long)auction.priceChange - 1);
                        auction.priceChange += 1;
                        
                        Clients.All.tokens(lastBidder.AspNetUser.numOfTokens, lastBidder.AspNetUser.Email);

                        db.Entry(lastBidder.AspNetUser).State = EntityState.Modified;
                        lastBidder.AspNetUser = user;
                        db.Entry(lastBidder).State = EntityState.Modified;
                    }
                }

                if (auction.duration < 10L)
                   auction.duration = 10L;

                db.Entry(user).State = EntityState.Modified;
                db.Entry(auction).State = EntityState.Modified;
                Bid bid = new Bid();
                bid.Auction = auction;
                bid.AspNetUser = user;
                bid.Date = DateTime.Now;
                db.Bids.Add(bid);
               
                db.SaveChanges();
                TimeSpan time = TimeSpan.FromSeconds(auction.duration);
                String timeS = time.ToString(@"dd\:hh\:mm\:ss");
                Clients.All.timerTick(auction.IdAuction, auction.stage, timeS, userEmail, (auction.price + auction.priceChange));
                Clients.All.tokens(user.numOfTokens, userEmail);
            }
        }

    }

    public class MyRegistry : Registry
    {
        private readonly object _lock = new object();

        public MyRegistry()
        {

            Schedule(() =>
            {

                // Check if some auction is over each 1 sec
               
                    Model1 db = new Model1();
                    var hubContext = GlobalHost.ConnectionManager.GetHubContext<AuctionsHub>();

                    var auctions = from o
                                    in db.Auctions
                                    where o.stage == "OPEN"
                                    select o;
                    

                    var auctionsList = auctions.ToList();
             

                    LastBidder lastBidder;
                    String bidderEmail;
               
                    foreach (var auction in auctionsList)
                    {
                    lock (mutex.locky)
                    {
                        lastBidder = null;
                        bidderEmail = "Be the first one to bid!";
                        var auctionChange = db.Auctions.Find(auction.IdAuction);
                        auctionChange.duration -= 1;

                        if (auctionChange.LastBidders.Count > 0)
                        {
                            lastBidder = auctionChange.LastBidders.First();
                            bidderEmail = lastBidder.AspNetUser.Email;
                        }

                      
                        if (auctionChange.duration == 0)
                        {

                            if (auctionChange.LastBidders.Count == 0)
                            {
                                auctionChange.stage = "EXPIRED";
                                auctionChange.dateClosed = DateTime.Now;
                            }
                            else {
                                auctionChange.stage = "SOLD";
                                auctionChange.dateClosed = DateTime.Now;
                                lastBidder.Status_ = "SOLD";
                                db.Entry(lastBidder).State = EntityState.Modified;
                            }
                        }
                        db.Entry(auctionChange).State = EntityState.Modified;
                        db.SaveChanges();

                        TimeSpan time = TimeSpan.FromSeconds(auctionChange.duration);
                        String timeS = time.ToString(@"dd\:hh\:mm\:ss");
                        long price = auctionChange.price + (long)auctionChange.priceChange;

                        hubContext.Clients.All.timerTick(auctionChange.IdAuction,auctionChange.stage, timeS, bidderEmail, price);
                    }
                }

                
            }).ToRunNow().AndEvery(1).Seconds();
        }

    }
}