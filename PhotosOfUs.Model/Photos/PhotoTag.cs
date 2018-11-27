using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.Models
{
    public partial class PhotoTag
    {
        private PhotoTag()
        {}

        public PhotoTag(int photoId, string tag)
        {
            PhotoId = photoId;
            Tag = new Tag(tag);
            RegisterDate = DateTime.UtcNow;
        }

        public int PhotoId { get; set; }
        public int TagId { get; set; }
        public DateTime RegisterDate { get; set; }

        public virtual Photo Photo { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
