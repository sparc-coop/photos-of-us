using System;
using System.Collections.Generic;

namespace PhotosOfUs.Model.Models
{
    public partial class Tag
    {
        public Tag()
        {
            PhotoTag = new HashSet<PhotoTag>();
        }

        public Tag(string name)
        {
            Name = name.Trim().ToLower();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<PhotoTag> PhotoTag { get; set; }
    }
}
