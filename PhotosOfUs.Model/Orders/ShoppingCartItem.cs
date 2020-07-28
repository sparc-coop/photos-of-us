using PhotosOfUs.Core.Photos;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Core.Orders
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }
        public string CartCode { get; set; }
        public int UserId { get; set; }
        public int PhotoId { get; set; }
        public int Quantity { get; set; }
        public DateTime DateCreatedUtc { get; set; }
        public virtual Photo Photo { get; set; }
    }
}
