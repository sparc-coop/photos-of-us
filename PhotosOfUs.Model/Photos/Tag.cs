using System;
using System.Collections.Generic;

namespace PhotosOfUs.Core.Photos
{
    public partial class Tag
    {
        public Tag()
        {

        }

        public Tag(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public int Id { get; set; }
        public string Name { get; set; }
    }
}
