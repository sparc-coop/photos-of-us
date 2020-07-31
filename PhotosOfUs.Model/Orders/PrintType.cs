using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Core.Orders
{
    public partial class PrintType
    {
        private decimal baseCost;

        public PrintType(string type, string height, string length, string icon, decimal baseCost)
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
        public decimal BaseCost
        {
            get => baseCost; protected set
            {
                if (baseCost <= 0)
                {
                    throw new ArgumentException("Base cost should be higher than 0");
                }
                baseCost = value;
            }
        }

        //public PrintPrice PrintPrice { get; protected set; }
        //public ICollection<OrderDetail> OrderDetail { get; protected set; }
    }
}
