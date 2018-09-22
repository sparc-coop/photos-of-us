﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

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
        public bool? IsDeactivated { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Dribbble { get; set; }
        public int TemplateSelected { get; set; }

        public ICollection<SocialMedia> SocialMedia { get; set; }
        public ICollection<Card> Card { get; set; }
        public ICollection<Folder> Folder { get; set; }
        public ICollection<Order> Order { get; set; }
        public ICollection<Photo> Photo { get; set; }
        public ICollection<PrintPrice> PrintPrice { get; set; }
        public Address Address { get; set; }

        public Claim[] GenerateClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
                new Claim(ClaimTypes.Email, Email),
                new Claim(ClaimTypes.Role, IsPhotographer == true ? "Photographer" : "Customer"),
                new Claim("userid", Id.ToString())
            };

            return claims.ToArray();
        }

        public void SetAddress(Address address)
        {
            address.UserId = Id;
            Address = address;
        }

        public void AddNewCards(int quantity)
        {
            for (var i = 0; i < quantity; i++)            
                Card.Add(new Card(this));
        }

        public Folder AddFolder(string name)
        {
            var folder = new Folder
            {
                Name = name,
                CreatedDate = DateTime.Now,
                PhotographerId = Id
            };
            Folder.Add(folder);
            return folder;
        }

        public void RemoveFolder(int folderId)
        {
            var folder = Folder.FirstOrDefault(x => x.Id == folderId);
            if (folder != null) folder.IsDeleted = true;
        }

        public Folder PublicFolder => Folder.FirstOrDefault(x => x.Name.ToLower() == "public");
    }
}
