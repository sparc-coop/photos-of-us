using System;
using System.Collections.Generic;

namespace PhotosOfUs.Model.Models
{
    public partial class Tag
    {
        //public Tag()
        //{
        //    Photo = new HashSet<Photo>();
        //}

        public int Id { get; set; }
        public string TagName { get; set; }

        //public ICollection<Photo> Photo { get; set; }
    }
}
