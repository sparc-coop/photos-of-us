using PhotosOfUs.Core.Photos;
using System;
using System.Collections.Generic;

namespace PhotosOfUs.Core.Orders
{
    public partial class OrderDetail
    {
        public OrderDetail(Order order, Photo photo, PrintType printType, int quantity)
        {
            OrderId = order.Id;
            PhotoId = photo.Id;
            PrintTypeId = printType.Id;
            Quantity = quantity;
            UnitPrice = (decimal)(photo.Price ?? 0) + (decimal)printType.BaseCost;
        }

        public int Id { get; protected set; }
        public int OrderId { get; protected set; }
        public int PhotoId { get; protected set; }
        public int Quantity { get; protected set; }
        public int? PrintTypeId { get; protected set; }
        public decimal UnitPrice { get; protected set; }
        public PrintType PrintType { get; protected set; }
    }
}
