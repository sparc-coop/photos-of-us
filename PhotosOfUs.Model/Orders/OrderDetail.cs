using System;
using System.Collections.Generic;

namespace PhotosOfUs.Model.Models
{
    public partial class OrderDetail
    {
        public OrderDetail()
        {}
        
        public OrderDetail(Photo photo, PrintType printType, int quantity)
        {
            PhotoId = photo.Id;
            PrintTypeId = printType.Id;
            Quantity = quantity;
            UnitPrice = (decimal)(photo.Price ?? 0) + (decimal)printType.BaseCost;
        }

        public int Id { get; set; }
        public int OrderId { get; set; }
        public int PhotoId { get; set; }
        public int Quantity { get; set; }
        public int? PrintTypeId { get; set; }
        public decimal UnitPrice { get; set; }
        public PrintType PrintType { get; set; }
    }
}
