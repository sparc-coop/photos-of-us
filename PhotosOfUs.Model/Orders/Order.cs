using PhotosOfUs.Core.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace PhotosOfUs.Model.Models
{
    public partial class Order
    {
        private int userId;
        private DateTime orderDate;

        public Order()
        {
            _orderDetails = new HashSet<OrderDetail>();
        }

        public Order(int userId, int? shippingAddressId, int? billingAddressId, decimal? total, Address billingAddress, Address shippingAddress)
        {
            UserId = userId;
            Status = OrderStatus.Open;
            OrderDateUtc = DateTime.UtcNow;

            ShippingAddressId = shippingAddressId;
            BillingAddressId = billingAddressId;
            Total = total;
            BillingAddress = billingAddress ?? throw new ArgumentNullException(nameof(billingAddress));
            ShippingAddress = shippingAddress ?? throw new ArgumentNullException(nameof(shippingAddress));
            _orderDetails = new HashSet<OrderDetail>();
        }

        public int Id { get; protected set; }
        public int UserId
        {
            get => userId;
            protected set
            {
                if (value <= 0)
                {
                    throw new ArgumentException();
                }
                userId = value;
            }
        }
        public int? ShippingAddressId { get; protected set; }
        public int? BillingAddressId { get; protected set; }
        public decimal? Total { get; protected set; }
        public OrderStatus Status { get; protected set; }
        public DateTime OrderDateUtc
        {
            get => orderDate;
            protected set
            {
                if (value == DateTime.MinValue)
                {
                    throw new ArgumentException();
                }
                orderDate = value;
            }
        }

        public Address BillingAddress { get; protected set; }
        public Address ShippingAddress { get; protected set; }

        private readonly HashSet<OrderDetail> _orderDetails;
        public IReadOnlyCollection<OrderDetail> OrderDetails => _orderDetails;

        // Not Mapped
        public int Amount => _orderDetails.First().Quantity;
        public decimal TotalPaid => _orderDetails.First().UnitPrice * Amount;
        public decimal Earning => TotalPaid * (decimal)0.955;


        public void AddLine(Photo photo, PrintType printType, int quantity)
        {
            _orderDetails.Add(new OrderDetail(this, photo, printType, quantity));
        }

        public decimal? CalculatedTotal => _orderDetails?.Sum(x => x.UnitPrice * x.Quantity);


    }
}
