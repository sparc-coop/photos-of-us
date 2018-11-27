using System;
using System.Collections.Generic;

namespace PhotosOfUs.Model.Models
{
    public partial class Tag
    {
        private Tag()
        {}
        
        public Tag(string name)
        {
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<PhotoTag> PhotoTag { get; set; }
    }
}
