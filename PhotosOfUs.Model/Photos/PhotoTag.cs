using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Core.Photos
{
    public partial class PhotoTag
    {
        private PhotoTag()
        {}

        public PhotoTag(int photoId, string tag)
        {
            PhotoId = photoId;
            Tag = new Tag(tag);
            RegisterDateUtc = DateTime.UtcNow;
        }

        public int PhotoId { get; set; }
        public int TagId { get; set; }
        public DateTime RegisterDateUtc { get; set; }

        public virtual Photo Photo { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
