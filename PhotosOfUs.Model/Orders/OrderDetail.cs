using PhotosOfUs.Core.Photos;
using System;
using System.Collections.Generic;

namespace PhotosOfUs.Core.Orders
{
    public partial class OrderDetail
    {
        private int quantity;
        private decimal unitPrice;

        private OrderDetail()
        {

        }

        public OrderDetail(Order order, Photo photo, PrintType printType, int quantity)
        {
            OrderId = order.Id;
            PhotoId = photo.Id;
            PrintTypeId = printType.Id;
            Quantity = quantity;
            UnitPrice = photo.Price + printType.BaseCost;
        }

        public int Id { get; protected set; }
        public int OrderId { get; protected set; }
        public int PhotoId { get; protected set; }
        public int Quantity
        {
            get => quantity; protected set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Quantity should be higher than 0");
                }
                quantity = value;
            }
        }
        public int PrintTypeId { get; protected set; }
        public decimal UnitPrice
        {
            get => unitPrice; protected set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("Unit Price should be higher than 0");
                }
                unitPrice = value;
            }
        }
        public PrintType PrintType { get; protected set; }
    }
}
