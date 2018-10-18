using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.Models
{
    public partial class PhotoTag
    {
        public int PhotoId { get; set; }
        public int TagId { get; set; }
        public DateTime RegisterDate { get; set; }

        public virtual Photo Photo { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
