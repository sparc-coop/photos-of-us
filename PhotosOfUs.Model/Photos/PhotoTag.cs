using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhotosOfUs.Model.Models
{
    public partial class PhotoTag
    {
        public int PhotoId { get; set; }
        public int TagId { get; set; }
        public DateTime? RegisterDate { get; set; }

        public Photo Photo { get; set; }
        public Tag Tag { get; set; }
    }
}
