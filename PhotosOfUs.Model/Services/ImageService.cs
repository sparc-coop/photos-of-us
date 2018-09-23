using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;

namespace PhotosOfUs.Model.Services
{
    public class ImageService
    {
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
                    originalImage.Save(output, GetImageFormatFromExtension(extension));
                }
                else
                {
                    thumbnailImage = originalImage.GetThumbnailImage(width, height, () => true, IntPtr.Zero);
                    thumbnailImage.Save(output, GetImageFormatFromExtension(extension));
                }
            }
            finally
            {
                thumbnailImage?.Dispose();
            }
            
            return output;
        }

        private ImageFormat GetImageFormatFromExtension(string extension)
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
                image.Save(output,GetImageFormatFromExtension(extension));
            }

            return output;
        }
    }
}
