namespace auctions123.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Order")]
    public partial class Order
    {
        [Key]
        public int IdOrder { get; set; }

        public int NumOfTokens { get; set; }

        [Required]
        [StringLength(128)]
        public string IdUser { get; set; }

        public double Price { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }

        public DateTime dateCreated { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }
    }
}
