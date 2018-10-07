using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;

namespace PhotosOfUs.Model.Models
{
    public partial class Photo : IFile
    {
        public Photo()
        {
            Tag = new HashSet<Tag>();
            PrintType = new HashSet<PrintType>();
            PhotoTag = new HashSet<PhotoTag>();
        }

        public Photo(int userId, string photoName, string extension, Stream stream, string photoCode, int folderId, double? price)
        {
            var urlTimeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            Filename = $"{userId}/profile/{photoName.Split('.')[0] + urlTimeStamp + extension}";
            FolderName = "photos";
            Stream = stream;
            Code = photoCode;
            FolderId = folderId;
            Price = (decimal)price;
        }

        public int Id { get; set; }
        public int PhotographerId { get; set; }
        public int FolderId { get; set; }
        public string Url { get; set; }
        public string Code { get; set; }
        public decimal? Price { get; set; }
        public string Name { get; set; }
        public DateTime UploadDate { get; set; }
        public bool PublicProfile { get; set; }
        public bool IsDeleted { get; set; }
        public RootObject SuggestedTags { get; set; }

        public Folder Folder { get; set; }
        public User Photographer { get; set; }

        public ICollection<PhotoTag> PhotoTag { get; set; }
        public ICollection<PrintType> PrintType { get; set; }
        public ICollection<Tag> Tag { get; set; }

        public string Filename { get; }
        public string FolderName { get; protected set; }
        public Stream Stream { get; protected set; }

        public ICollection<PrintType> GetPrintTypes()
        {
            return PrintType;
        }

        public void UpdatePrice(decimal newPrice)
        {
            Price = newPrice;
        }

        public ICollection<Tag> GetAllTags()
        {
            return Tag;
        }

        public void Delete()
        {
            IsDeleted = true;
        }

/*         public PhotoTag NewPhotoTag(PhotoTag tag)
        {
            PhotoTag. = photoid;
            TagId = tagtoid.Id,
            RegisterDate = DateTime.Now
        } */

        public Tag NewTag(Tag newTag)
        {
            var tag = new Tag
            {
                Name = newTag.Name,
                Id = newTag.Id
            };
            Tag.Add(tag);
            return tag;
        }
    }
}
