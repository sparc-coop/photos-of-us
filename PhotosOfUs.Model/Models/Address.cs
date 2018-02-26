using System;
using System.Collections.Generic;

namespace PhotosOfUs.Model.Models
{
    public partial class Address
    {
        public Address()
        {
            OrderBillingAddress = new HashSet<Order>();
            OrderShippingAddress = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string FullName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }

        public ICollection<Order> OrderBillingAddress { get; set; }
        public ICollection<Order> OrderShippingAddress { get; set; }
    }
}
