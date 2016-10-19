namespace auctions123.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Bid")]
    public partial class Bid
    {
        [Key]
        public int IdBid { get; set; }

        [Required]
        [StringLength(128)]
        public string IdUser { get; set; }

        public int IdAuction { get; set; }

        public DateTime Date { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual Auction Auction { get; set; }
    }
}
