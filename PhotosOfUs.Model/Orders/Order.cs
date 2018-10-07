﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace PhotosOfUs.Model.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetail = new HashSet<OrderDetail>();
        }

        public Order(int userId)
        {
            OrderStatus = "Open";
            UserId = userId;
            OrderDate = DateTime.Now;
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

        public void AddLine(Photo photo, PrintType printType, int quantity)
        {
            OrderDetail.Add(new OrderDetail(photo, printType, quantity));
        }

        public void SetStatusToPending()
        {
            OrderStatus = "Pending";
        }


    }
}