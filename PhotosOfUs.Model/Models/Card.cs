using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.Models
{
    public partial class Card
    {
        public int Id { get; set; }
        public int PhotographerId { get; set; }        
        public string Code { get; set; }
        public DateTime CreatedDate { get; set; }

        public User Photographer { get; set; }
    }
}
