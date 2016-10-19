namespace auctions123.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LastBidder")]
    public partial class LastBidder
    {
        [Key]
        public int IdLastBid { get; set; }

        public int IdAuction { get; set; }

        [Required]
        [StringLength(128)]
        public string IdUser { get; set; }

        [Column("Status ")]
        [StringLength(50)]
        public string Status_ { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual Auction Auction { get; set; }
    }
}
