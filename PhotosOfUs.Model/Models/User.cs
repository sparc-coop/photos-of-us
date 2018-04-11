using System;
using System.Collections.Generic;

namespace PhotosOfUs.Model.Models
{
    public partial class User
    {
        public User()
        {
            Card = new HashSet<Card>();
            Folder = new HashSet<Folder>();
            Order = new HashSet<Order>();
            Photo = new HashSet<Photo>();
        }

        public int Id { get; set; }
        public string AzureId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string JobPosition { get; set; }
        public string Bio { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool? IsPhotographer { get; set; }
        public string Facebook { get; set; }

        public ICollection<Card> Card { get; set; }
        public ICollection<Folder> Folder { get; set; }
        public ICollection<Order> Order { get; set; }
        public ICollection<Photo> Photo { get; set; }
        public ICollection<PrintPrice> PrintPrice { get; set; }
        public Address Address { get; set; }
    }
}
