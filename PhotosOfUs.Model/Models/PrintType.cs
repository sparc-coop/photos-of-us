using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.Models
{
    public partial class PrintType
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Height { get; set; }
        public string Length { get; set; }
        public string Icon { get; set; }

        public ICollection<OrderDetail> OrderDetail { get; set; }
    }
}
