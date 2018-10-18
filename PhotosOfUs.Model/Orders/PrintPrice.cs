using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.Models
{
    public partial class PrintPrice
    {
        public int Id { get; set; }
        public int PhotoId { get; set; }
        public decimal Price { get; set; }
        public int PhotographerId { get; set; }

        public User Photographer { get; set; }
    }
}
