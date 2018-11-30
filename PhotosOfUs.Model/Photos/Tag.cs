using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhotosOfUs.Model.Models
{
    public partial class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<PhotoTag> PhotoTags { get; set; }
    }
}
