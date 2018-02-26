using System;
using System.Collections.Generic;

namespace PhotosOfUs.Model.Models
{
    public partial class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int PhotoId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public Order Order { get; set; }
        public Photo Photo { get; set; }
    }
}
