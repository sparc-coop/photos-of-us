using System;
using System.Collections.Generic;

namespace PhotosOfUs.Model.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetail = new HashSet<OrderDetail>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public int? ShippingAddressId { get; set; }
        public int? BillingAddressId { get; set; }
        public decimal? Total { get; set; }
        public string OrderStatus { get; set; } // known possible values: ["Order Complete", "Payment Pending"]
        public DateTime OrderDate { get; set; }

        public Address BillingAddress { get; set; }
        public Address ShippingAddress { get; set; }
        public User User { get; set; }
        public ICollection<OrderDetail> OrderDetail { get; set; }
    }
}
