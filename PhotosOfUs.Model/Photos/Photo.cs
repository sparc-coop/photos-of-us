using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace PhotosOfUs.Model.Models
{
    public partial class Photo : IFile
    {
        public Photo()
        {
            PrintType = new HashSet<PrintType>();
            PhotoTag = new HashSet<PhotoTag>();
        }

        public Photo(int userId, string photoName, string extension, Stream stream, string photoCode, int folderId, double? price, string url, bool publicProfile = false)
        {
            Name = photoName;
            var urlTimeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            Filename = $"{userId}/profile/{photoName.Split('.')[0] + urlTimeStamp + extension}";
            FolderName = "photos";
            Stream = stream;
            Code = photoCode;
            FolderId = folderId;
            Price = (decimal)price;
            UploadDate = DateTime.Now;
            Url = url;
            PhotographerId = userId;
            PublicProfile = publicProfile;
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

        [NotMapped]
        public string Filename { get; set; }
        [NotMapped]
        public string FolderName { get; set; }
        [NotMapped]
        public string FileSize { get; protected set; }
        [NotMapped]
        public string Resolution { get; protected set; }
        [NotMapped]
        public Stream Stream { get; set; }
        [NotMapped]
        public string ThumbnailUrl => Url?.Replace("/photos/", "/thumbnails/");
        [NotMapped]
        public string WaterMarkUrl => Url?.Replace("/photos/", "/watermark/");



        public ICollection<PrintType> GetPrintTypes()
        {
            return PrintType;
        }

        public void UpdatePrice(decimal newPrice)
        {
            Price = newPrice;
        }

        //public ICollection<Tag> GetAllTags()
        //{
        //    return Tag;
        //}

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

        //public Tag NewTag(Tag newTag)
        //{
        //    var tag = new Tag
        //    {
        //        Name = newTag.Name,
        //        Id = newTag.Id
        //    };
        //    Tag.Add(tag);
        //    return tag;
        //}

        public Stream AddWatermark(Stream input, string extension)
        {
            var output = new MemoryStream();
            
            WebClient wc = new WebClient();
            byte[] bytes = wc.DownloadData("https://photosofus-dev.azurewebsites.net/images/water_mark.png");
            MemoryStream ms = new MemoryStream(bytes);

            using (Image image = Image.FromStream(input))
            using (Image watermarkImage = Image.FromStream(ms))
            using (Graphics imageGraphics = Graphics.FromImage(image))
            using (TextureBrush watermarkBrush = new TextureBrush(watermarkImage))
            {
                int x = (image.Width / 2 - watermarkImage.Width / 2);
                int y = (image.Height / 2 - watermarkImage.Height / 2);
                watermarkBrush.TranslateTransform(x, y);
                imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(x, y), new Size(watermarkImage.Width + 1, watermarkImage.Height)));
                //imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(0, 0), image.Size));
                image.Save(output, GetImageFormatFromExtension(extension));
            }

            return output;
        }

        public static ImageFormat GetImageFormatFromExtension(string extension)
        {
            if (extension.ToLower() == "png")
            {
                return ImageFormat.Png;
            }
            else if (extension.ToLower() == "tiff" || extension.ToLower() == "tif")
            {
                return ImageFormat.Tiff;
            }
            else if (extension.ToLower() == "bmp")
            {
                return ImageFormat.Bmp;
            }
            else if (extension.ToLower() == "jpg" || extension.ToLower() == "jpeg" || extension.ToLower() == "jpe")
            {
                return ImageFormat.Png;
            }
            else
            {
                return ImageFormat.Png;
            }
        }

        public Stream ConvertToThumbnail(Stream image, string extension, int thumbnailSize)
        {
            image.Position = 0;
            var output = new MemoryStream();
            int width;
            int height;
            var originalImage = new Bitmap(image);

            if (originalImage.Width > originalImage.Height)
            {
                width = thumbnailSize;
                height = thumbnailSize * originalImage.Height / originalImage.Width;
            }
            else
            {
                height = thumbnailSize;
                width = thumbnailSize * originalImage.Width / originalImage.Height;
            }

            Image thumbnailImage = null;
            try
            {
                if (height > originalImage.Height || width > originalImage.Width)
                {
                    originalImage.Save(output, Photo.GetImageFormatFromExtension(extension));
                }
                else
                {
                    thumbnailImage = originalImage.GetThumbnailImage(width, height, () => true, IntPtr.Zero);
                    thumbnailImage.Save(output, Photo.GetImageFormatFromExtension(extension));
                }
            }
            finally
            {
                thumbnailImage?.Dispose();
            }

            return output;
        }
    }
}
