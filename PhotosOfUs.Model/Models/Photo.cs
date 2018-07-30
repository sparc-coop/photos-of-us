using System;
using System.Collections.Generic;

namespace PhotosOfUs.Model.Models
{
    public partial class Photo
    {
        public Photo()
        {
            OrderDetail = new HashSet<OrderDetail>();
            //Tag = new HashSet<Tag>();
        }

        public int Id { get; set; }
        public int PhotographerId { get; set; }
        public int FolderId { get; set; }
        public string Url { get; set; }
        public string Code { get; set; }
        public decimal? Price { get; set; }
        public string Name { get; set; }
        public DateTime UploadDate { get; set; }
        public bool PublicProfile { get; set; }

        public Folder Folder { get; set; }
        public User Photographer { get; set; }
        public ICollection<OrderDetail> OrderDetail { get; set; }

        public ICollection<PhotoTag> PhotoTag { get; set; }
    }
}
