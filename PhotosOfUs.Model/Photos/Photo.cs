﻿using Kuvio.Kernel.Core;
using Kuvio.Kernel.Core.Common;
using PhotosOfUs.Core.Events;
using PhotosOfUs.Core.Users;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace PhotosOfUs.Core.Photos
{
    public partial class Photo : IFile
    {
        public Photo()
        {
            _photoTag = new HashSet<PhotoTag>();
        }

        public Photo(int userId, string filename, int eventId, int? cardId, Stream stream)
        {
            var urlTimeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var extension = filename.Contains('.') ? filename.Split('.').Last() : ".jpg";
            Filename = $"{userId}/{filename.Split('.')[0] + urlTimeStamp + extension}";
            Name = $"{filename.Split('.')[0]}";
            Stream = stream;
            UploadDateUtc = DateTime.UtcNow;
            PhotographerId = userId;
            _photoTag = new HashSet<PhotoTag>();

            EventId = eventId;
            CardId = cardId;
            Filename = Filename.Replace($"{userId}/", $"{userId}/{eventId}/");
        }

        public int Id { get; set; }
        public int PhotographerId { get; set; }
        public string Url { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; }
        public DateTime UploadDateUtc { get; set; }
        public bool PublicProfile { get; set; }
        public bool IsDeleted { get; set; }
        public int? EventId { get; set; }
        public Event Event { get; set; }
        public int? CardId { get; set; }

        public User Photographer { get; set; }


        private readonly HashSet<PhotoTag> _photoTag;
        public IReadOnlyCollection<PhotoTag> PhotoTag => _photoTag;


        public string Filename { get; }

        // Not Mapped
        public string FolderName { get; protected set; } = "photos";
        public string FileSize { get; protected set; }
        public string Resolution { get; protected set; }
        public Stream Stream { get; protected set; }
        public string ThumbnailUrl => Url?.Replace("/photos/", "/thumbnails/");
        public string WaterMarkUrl => Url?.Replace("/photos/", "/watermark/");

        public void UpdatePrice(decimal newPrice)
        {
            Price = newPrice;
        }

        public IEnumerable<Tag> GetAllTags()
        {
            return PhotoTag.Select(x => x.Tag);
        }

        public void ReplaceTags(List<string> tags)
        {
            foreach (var tag in _photoTag.Where(x => !tags.Contains(x.Tag.Name)).ToList())
            {
                _photoTag.Remove(tag);
            }
                
            foreach (var tag in tags.Where(x => !_photoTag.Any(y => y.Tag.Name == x)))
            {
                _photoTag.Add(new PhotoTag(Id, tag));
            }
        }

        public void Delete()
        {
            IsDeleted = true;
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
                return ImageFormat.Jpeg;
            }
            else
            {
                return ImageFormat.Png;
            }
        }
    }
}
