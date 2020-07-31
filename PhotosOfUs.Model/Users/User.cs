﻿using PhotosOfUs.Core.Orders;
using PhotosOfUs.Core.Photos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace PhotosOfUs.Core.Users
{
    public partial class User
    {
        private User()
        {
            //Folders = new HashSet<Folder>();
            _userIdentities = new HashSet<UserIdentity>();
        }
        
        public User(string firstName, string lastName, string email, Role role, string externalUserId)
        {
            FirstName = firstName;
            LastName = lastName;
            Role = role;
            Email = email;
            CreateDateUtc = DateTime.UtcNow;
            AzureId = externalUserId;

            _userIdentities = new HashSet<UserIdentity>();
            if (!String.IsNullOrWhiteSpace(externalUserId))
            {
                GetOrCreateIdentity(externalUserId);
            }
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
        public Role Role { get; protected set; }


        public DateTime CreateDateUtc { get; private set; }
        public DateTime? LastLoginDateUtc { get; set; }
        public bool? IsPhotographer { get; private set; }
        
        public bool? IsDeactivated { get; private set; }
        public string Facebook { get; private set; }
        public string Twitter { get; private set; }
        public string Instagram { get; private set; }
        public string Dribbble { get; private set; }
        public int TemplateSelected { get; private set; }
        public bool? PurchaseTour { get; set; }
        public bool? DashboardTour { get; set; }
        public bool? PhotoTour { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        //public ICollection<Folder> Folders { get; set; }
        //public ICollection<Order> Order { get; set; }
        //public ICollection<PrintPrice> PrintPrice { get; set; }
        private readonly HashSet<UserIdentity> _userIdentities;
        public IReadOnlyCollection<UserIdentity> UserIdentities => _userIdentities;
        public Address Address { get; set; }


        private List<Claim> GenerateClaims()
        {
            var claims = new List<Claim>
            {
                new Claim("ID", Id.ToString()),
                new Claim(ClaimTypes.Email, Email),
                new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
                new Claim(ClaimTypes.Role, Role.Value),
                new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", UserIdentities.First().AzureID),
                new Claim("Name", FullName)
            };

            if (!String.IsNullOrWhiteSpace(ProfilePhotoUrl))
            {
                claims.Add(new Claim("ProfilePictureUrl", ProfilePhotoUrl));
            }

            return claims;
        }

        private UserIdentity GetOrCreateIdentity(string externalUserId)
        {
            var identity = UserIdentities.SingleOrDefault(x => x.AzureID == externalUserId);

            if (identity == null)
            {
                identity = new UserIdentity(this, externalUserId, "Azure");
                _userIdentities.Add(identity);
            }
            return identity;
        }
        
        public void Login(ClaimsPrincipal principal, string externalUserId)
        {
            var identity = GetOrCreateIdentity(externalUserId);
            identity.LastLoginDateUtc = DateTime.UtcNow;

            List<Claim> claims = GenerateClaims();

            // Logging in is simply adding claims to the existing principal
            foreach (var claim in claims.Where(x => !principal.HasClaim(y => y.Type == x.Type)))
            {
                (principal.Identity as ClaimsIdentity)?.AddClaim(claim);
            }
        }

        public void UpdateProfile(string email, string firstName, string lastName, string displayName, string jobPosition, string profilePhotoUrl, string bio)
        {
            if(email != null)
                Email = email;

            LastName = lastName;
            DisplayName = displayName;
            FirstName = firstName;
            JobPosition = jobPosition;
            Bio = bio;
            SetProfilePhoto(profilePhotoUrl);
        }

        public void SetProfilePhoto(string absoluteUri)
        {
            ProfilePhotoUrl = absoluteUri;
        }

        public void UpdateSocialMedia(string facebook, string instagram, string dribbble, string twitter)
        {
            Facebook = facebook;
            Instagram = instagram;
            Dribbble = dribbble;
            Twitter = twitter;
        }

        public void Deactivate()
        {
            IsDeactivated = true;
        }

        public void Activate()
        {
            IsDeactivated = false;
        }

        public void SetAddress(Address address)
        {
            Address = address;
        }

        //public Folder AddFolder(string name)
        //{
        //    var folder = new Folder(this, name);
        //    Folders.Add(folder);
        //    return folder;
        //}

        //public void RemoveFolder(int folderId)
        //{
        //    var folder = Folders.FirstOrDefault(x => x.Id == folderId);
        //    if (folder != null) folder.IsDeleted = true;
        //}

        //public Folder PublicFolder => Folders.FirstOrDefault(x => x.Name.ToLower() == "public");
    }
}
