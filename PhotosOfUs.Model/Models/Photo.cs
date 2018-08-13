using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhotosOfUs.Model.Models
{
    public partial class Photo
    {
        public Photo()
        {
            //OrderDetail = new HashSet<OrderDetail>();
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
        public bool IsDeleted { get; set; }
        [Column("SuggestedTags")]
        public string SuggestedTagsRaw { get; set; }

        public Folder Folder { get; set; }
        public User Photographer { get; set; }
        //public ICollection<OrderDetail> OrderDetail { get; set; }

        public ICollection<PhotoTag> PhotoTag { get; set; }

        public RootObject SuggestedTags
        {
            get => JsonConvert.DeserializeObject<RootObject>(SuggestedTagsRaw);
            set => SuggestedTagsRaw = JsonConvert.SerializeObject(value);
        }
    }
}
