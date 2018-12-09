using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace PhotosOfUs.Model.Models
{
    public partial class User
    {
        public User()
        {
            Folders = new HashSet<Folder>();
            Order = new HashSet<Order>();
            Photos = new HashSet<Photo>();
            UserIdentities = new HashSet<UserIdentity>();
        }
        
        public User(string displayName, string email, string externalUserId, bool isPhotographer)
        {
            DisplayName = displayName;
            FirstName = displayName?.Split(' ').First();
            LastName = displayName?.Split(' ').Last();
            IsPhotographer = isPhotographer;
            Email = email;
            CreateDate = DateTime.UtcNow;
            AzureId = "e95fe8d9-82f4-4dfe-aa9b-0f364ccd0a90";

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

        public DateTime CreateDate { get; private set; }
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

        public string FullName => $"{(FirstName == null ? DisplayName : FirstName)}{(LastName == null ? "" : (" " + LastName))}";

        public ICollection<SocialMedia> SocialMedia { get; set; }
        public ICollection<Folder> Folders { get; set; }
        public ICollection<Order> Order { get; set; }
        public ICollection<Photo> Photos { get; set; }
        public ICollection<PrintPrice> PrintPrice { get; set; }
        public ICollection<UserIdentity> UserIdentities { get; set; }
        public Address Address { get; set; }

        public List<Claim> GenerateClaims()
        {
            return new List<Claim>
            {
                new Claim("ID", Id.ToString()),
                new Claim(ClaimTypes.Email, Email),
                new Claim(ClaimTypes.NameIdentifier, Id.ToString()),
                new Claim(ClaimTypes.Role, IsPhotographer == true ? "Photographer" : "Customer"),
                //new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", AzureId)
            };
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
            List<Claim> claims = GenerateClaims();

            // Logging in is simply adding claims to the existing principal
            foreach (var claim in claims.Where(x => !principal.HasClaim(y => y.Type == x.Type)))
            {
                (principal.Identity as ClaimsIdentity)?.AddClaim(claim);
            }
                
            var identity = GetOrCreateIdentity(externalUserId);
            identity.LastLoginDate = DateTime.UtcNow;
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
            address.UserId = Id;
            Address = address;
        }

        public Folder AddFolder(string name)
        {
            var folder = new Folder(name, Id);
            Folders.Add(folder);
            return folder;
        }

        public void RemoveFolder(int folderId)
        {
            var folder = Folders.FirstOrDefault(x => x.Id == folderId);
            if (folder != null) folder.IsDeleted = true;
        }

        public Folder PublicFolder => Folders.FirstOrDefault(x => x.Name.ToLower() == "public");
    }
}
