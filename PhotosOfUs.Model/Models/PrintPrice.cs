using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.Models
{
    public partial class PrintPrice
    {
        public int Id { get; set; }
        public int PrintTypeId { get; set; }
        public string Price { get; set; }
        public int PhotographerId { get; set; }

        //public PrintType PrintType { get; set; }
        public User Photographer { get; set; }
    }
}
