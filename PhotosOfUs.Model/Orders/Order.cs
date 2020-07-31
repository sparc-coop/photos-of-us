using PhotosOfUs.Core.Orders;
using PhotosOfUs.Core.Photos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace PhotosOfUs.Core.Orders
{
    public partial class Order
    {
        private Order()
        {
            _orderDetails = new HashSet<OrderDetail>();
        }

        public Order(int userId, Address billingAddress, Address shippingAddress)
        {
            UserId = userId;
            Status = OrderStatus.Open;
            OrderDateUtc = DateTime.UtcNow;

            ShippingAddressId = shippingAddress.Id;
            BillingAddressId = billingAddress.Id;
            BillingAddress = billingAddress ?? throw new ArgumentNullException(nameof(billingAddress));
            ShippingAddress = shippingAddress ?? throw new ArgumentNullException(nameof(shippingAddress));
            _orderDetails = new HashSet<OrderDetail>();
        }

        public int Id { get; protected set; }
        public int UserId { get; set; }
        public int ShippingAddressId { get; protected set; }
        public int BillingAddressId { get; protected set; }
        //public decimal? Total { get; protected set; }
        public OrderStatus Status { get; protected set; }
        private DateTime orderDateUtc;
        public DateTime OrderDateUtc
        {
            get => orderDateUtc;
            protected set
            {
                if (value == DateTime.MinValue)
                {
                    throw new ArgumentException();
                }
                orderDateUtc = value;
            }
        }

        public Address BillingAddress { get; protected set; }
        public Address ShippingAddress { get; protected set; }

        private readonly HashSet<OrderDetail> _orderDetails;
        public IReadOnlyCollection<OrderDetail> OrderDetails => _orderDetails;

        private decimal total;
        public decimal Total
        {
            get { return _orderDetails.Sum(x => x.UnitPrice * x.Quantity); }
            protected set // TODO: Test if this is working properly
            {
                total = _orderDetails.Sum(x => x.UnitPrice * x.Quantity);
            }
        }


        // Not Mapped
        public int Amount => _orderDetails.Sum(x => x.Quantity);
        public decimal Earning => Total * (decimal)0.955;


        public void AddLine(Photo photo, PrintType printType, int quantity)
        {
            _orderDetails.Add(new OrderDetail(this, photo, printType, quantity));
        }




    }
}
