using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Bookshop.Models
{
    /// <summary>
    /// Model to keep singular details about order.
    /// Foreign Keys pointing to User who placed order and
    /// Order_Lines table, which contains details on products ordered and quantity.
    /// </summary>
    public class Order
    {

        [Key]
        public int OrderId { get; set; }
        /// <summary>
        /// Might be better types to use here. Using DateTime for convenience, haven't thought about if more limited versions may serve just as well.
        /// </summary>
        public DateTime PlaceDate { get; set; }
        public DateTime? ShipDate { get; set; }

        public string? TrackingNumber { get; set; }
        public string OrderStatus { get; set; } = "Placed";

        [Required]
        public string Name { get; set; } = "Anonymous";

        /// <summary>
        /// Shipping Address
        /// </summary>
        [Required]
        public string? ShipStreetAddress { get; set; }
        [Required]
        public string? ShipCity { get; set; }
        [Required]
        public string? ShipState { get; set; }
        [Required]
        public string? ShipPostalCode { get; set; }

        /// <summary>
        /// Billing Address
        /// </summary>
        [Required]
        public string? BillStreetAddress { get; set; }
        [Required]
        public string? BillCity { get; set; }
        [Required]
        public string? BillState { get; set; }
        [Required]
        public string? BillPostalCode { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = "(XXX) XXX-XXXX";

        /// <summary>
        /// FK to User who placed order.
        /// </summary>
        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        [ValidateNever]
        public AppUser? User { get; set; }



    }
}
