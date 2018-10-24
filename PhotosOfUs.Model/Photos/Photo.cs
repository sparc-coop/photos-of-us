using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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

        // Not Mapped
        public string FolderName { get; protected set; }
        public string FileSize { get; protected set; }
        public string Resolution { get; protected set; }
        public Stream Stream { get; protected set; }
        public string ThumbnailUrl => Url?.Replace("/photos/", "/thumbnails/");
        public string WaterMarkUrl => Url?.Replace("/photos/", "/watermark/");



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

        public Stream AddWatermark(Stream input, Stream watermark, string extension)
        {
            var output = new MemoryStream();
            
            //WebClient wc = new WebClient();
            //byte[] bytes = wc.DownloadData("https://photosofus-dev.azurewebsites.net/images/water_mark.png");
            //MemoryStream ms = new MemoryStream(bytes);

            using (Image image = Image.FromStream(input))
            using (Image watermarkImage = Image.FromStream(watermark))
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
    }
}
