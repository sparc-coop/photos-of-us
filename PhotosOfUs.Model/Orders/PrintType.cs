using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Model.Models
{
    public partial class PrintType
    {
        public PrintType(string type, string height, string length, string icon, double baseCost)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Height = height ?? throw new ArgumentNullException(nameof(height));
            Length = length ?? throw new ArgumentNullException(nameof(length));
            Icon = icon ?? throw new ArgumentNullException(nameof(icon));
            BaseCost = baseCost;
        }

        public int Id { get; protected set; }
        public string Type { get; protected set; }
        public string Height { get; protected set; }
        public string Length { get; protected set; }
        public string Icon { get; protected set; }
        public double BaseCost { get; protected set; }
        
        //public PrintPrice PrintPrice { get; protected set; }
        //public ICollection<OrderDetail> OrderDetail { get; protected set; }
    }
}
