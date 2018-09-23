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
            UserIdentities = new HashSet<UserIdentity>();
        }
        
        public User(string displayName, string email, string externalUserId, bool isPhotographer)
        {
            this.DisplayName = displayName;
            this.FirstName = displayName?.Split(' ').First();
            this.LastName = displayName?.Split(' ').Last();
            this.IsPhotographer = isPhotographer;
            CreateDate = DateTime.UtcNow;

            this.UserIdentities = new List<UserIdentity> {
                new UserIdentity(externalUserId, "Azure")
            };
        }

        public int Id { get; private set; }
        public string AzureId { get; private set; }
        public string Email { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string DisplayName { get; private set; }
        public string JobPosition { get; private set; }
        public string Bio { get; private set; }
        public string ProfilePhotoUrl { get; private set; }

        public void UpdateSocialMedia(string facebook, string instagram, string dribbble, string twitter)
        {
            Facebook = facebook;
            Instagram = instagram;
            Dribbble = dribbble;
            Twitter = twitter;
        }

        public DateTime CreateDate { get; private set; }
        public bool? IsPhotographer { get; private set; }
        public bool? IsDeactivated { get; private set; }
        public string Facebook { get; private set; }
        public string Twitter { get; private set; }
        public string Instagram { get; private set; }
        public string Dribbble { get; private set; }
        public int TemplateSelected { get; private set; }

        public ICollection<SocialMedia> SocialMedia { get; private set; }
        public ICollection<Card> Card { get; private set; }
        public ICollection<Folder> Folder { get; private set; }
        public ICollection<Order> Order { get; private set; }
        public ICollection<Photo> Photo { get; private set; }
        public ICollection<PrintPrice> PrintPrice { get; private set; }
        public ICollection<UserIdentity> UserIdentities { get; private set; }
        public Address Address { get; private set; }

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
        
        public UserIdentity GetOrCreateIdentity(string externalUserId)
        {
            var identity = UserIdentities.SingleOrDefault(x => x.AzureID == externalUserId);
            if (identity == null)
            {
                identity = new UserIdentity(externalUserId, "Azure");
                UserIdentities.Add(identity);
            }
            return identity;
        }
        
        public void Login(ClaimsPrincipal principal, string externalUserId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
                new Claim(ClaimTypes.Email, Email),
                new Claim(ClaimTypes.Role, IsPhotographer == true ? "Photographer" : "Customer")
            };

            // Logging in is simply adding claims to the existing principal
            foreach (var claim in claims.Where(x => !principal.HasClaim(y => y.Type == x.Type)))
                (principal.Identity as ClaimsIdentity)?.AddClaim(claim);

            var identity = GetOrCreateIdentity(externalUserId);
            identity.LastLoginDate = DateTime.UtcNow;
        }

        public void UpdateProfile(string email, string firstName, string lastName, string displayName, string jobPosition, string profilePhotoUrl, string bio)
        {
            if(email != null)
                Email = email;

            LastName = lastName;
            DisplayName = displayName;
            ProfilePhotoUrl = profilePhotoUrl;
            FirstName = firstName;
            JobPosition = jobPosition;
            Bio = bio;
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
