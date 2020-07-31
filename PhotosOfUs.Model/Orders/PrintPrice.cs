using PhotosOfUs.Core.Photos;
using PhotosOfUs.Core.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotosOfUs.Core.Orders
{
    public partial class PrintPrice
    {
        private decimal price;

        private PrintPrice()
        {

        }

        public PrintPrice(decimal price, Photo photo, User photographer)
        {
            PhotoId = photo.Id;
            Price = price;
            PhotographerId = photographer.Id;

            if (photo.PhotographerId != Photographer.Id)
            {
                throw new ArgumentException("Photo is not owned by this photographer.");
            }
        }

        public int Id { get; protected set; }
        public int PhotoId { get; protected set; }
        public decimal Price
        {
            get => price; 
            protected set
            {
                if (value <= 0)
                {
                    throw new ArgumentException();
                }
                price = value;
            }
        }
        public int PhotographerId { get; protected set; }

        public User Photographer { get; protected set; }
    }
}
