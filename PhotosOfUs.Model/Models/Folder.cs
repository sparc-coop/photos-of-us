using System;
using System.Collections.Generic;

namespace PhotosOfUs.Model.Models
{
    public partial class Folder
    {
        public Folder()
        {
            Photo = new HashSet<Photo>();
        }

        public int Id { get; set; }
        public int PhotographerId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }

        public User Photographer { get; set; }
        public ICollection<Photo> Photo { get; set; }
    }
}
