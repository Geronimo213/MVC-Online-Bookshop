﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }

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
        [RegularExpression(@"^\d{5}(?:[-\s]\d{4})?$", ErrorMessage = "Not a valid zip code")]
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
        [RegularExpression(@"^\d{5}(?:[-\s]\d{4})?$", ErrorMessage = "Not a valid zip code")]
        public string? BillPostalCode { get; set; }

        [Required]
        [Phone]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        /// <summary>
        /// FK to User who placed order.
        /// </summary>
        [ForeignKey("User")]
        public string? UserId { get; set; }
        [ValidateNever]
        public AppUser? User { get; set; }



    }
}
