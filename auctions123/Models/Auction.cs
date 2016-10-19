namespace auctions123.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Auction")]
    public partial class Auction
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Auction()
        {
            Bids = new HashSet<Bid>();
            LastBidders = new HashSet<LastBidder>();
        }

        [Key]
        public int IdAuction { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Name")]
        public string productName { get; set; }

        [Required]
        //[RegularExpression("([0-9]+)", ErrorMessage = "Please enter valid integer Number")]
        [Range(0, long.MaxValue, ErrorMessage = "Please enter valid number")]
        [Display(Name = "Duration")]
        public long duration { get; set; }

        [Display(Name = "Date Created")]
        public DateTime dateCreated { get; set; }

        [Display(Name = "Date Opened")]
        public DateTime? dateOpened { get; set; }

        [Display(Name = "Date Closed")]
        public DateTime? dateClosed { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Stage")]
        public string stage { get; set; }

        [Required]
        [Display(Name = "Image")]
        public byte[] image { get; set; }

        [Required]
        //[RegularExpression("([0-9]+)", ErrorMessage = "Please enter valid integer Number")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid number")]
        [Display(Name = "Price")]
        // [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int price { get; set; }

        [Display(Name = "Price Change")]
        public int? priceChange { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Bid> Bids { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LastBidder> LastBidders { get; set; }
    }
}
