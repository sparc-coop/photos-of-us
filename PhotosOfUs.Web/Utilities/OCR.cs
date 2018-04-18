using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PhotosOfUs.Model.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tesseract;



namespace PhotosOfUs.Web.Utilities
{
    public class OCRModelResult
    {
        public float MeanConfidence { get; set; }
        public string Text { get; set; }
        public string Code { get; set; }
    }


    public class OCR
    {
        private IHostingEnvironment _environment;
        private PhotosOfUsContext _context;

        public OCR(PhotosOfUsContext context,IHostingEnvironment environment)
        {
            _environment = environment;
            _context = context;
        }

        public OCRModelResult GetOCRResult(IFormFile imageFile, int photographerId)
        {

            OCRModelResult model = new OCRModelResult();


            using (var engine = new TesseractEngine(Path.Combine(_environment.WebRootPath, "tessdata"), "eng", EngineMode.Default))
            {
                using (var image = new Bitmap(imageFile.OpenReadStream()))
                {

                    using (var page = engine.Process(SetGrayscale(RemoveNoise(Resize(image,image.Width*2,image.Height*2)))))
                    {
                        model.MeanConfidence = page.GetMeanConfidence();
                        model.Text = page.GetText();
                        model.Code = ExtractCode(model.Text,photographerId);

                        return model;
                    }

                }
            }
            
        }

        public Bitmap Resize(Bitmap bmp, int newWidth, int newHeight)
        {

            Bitmap temp = (Bitmap)bmp;

            Bitmap bmap = new Bitmap(newWidth, newHeight, temp.PixelFormat);

            double nWidthFactor = (double)temp.Width / (double)newWidth;
            double nHeightFactor = (double)temp.Height / (double)newHeight;

            double fx, fy, nx, ny;
            int cx, cy, fr_x, fr_y;
            Color color1 = new Color();
            Color color2 = new Color();
            Color color3 = new Color();
            Color color4 = new Color();
            byte nRed, nGreen, nBlue;

            byte bp1, bp2;

            for (int x = 0; x < bmap.Width; ++x)
            {
                for (int y = 0; y < bmap.Height; ++y)
                {

                    fr_x = (int)Math.Floor(x * nWidthFactor);
                    fr_y = (int)Math.Floor(y * nHeightFactor);
                    cx = fr_x + 1;
                    if (cx >= temp.Width) cx = fr_x;
                    cy = fr_y + 1;
                    if (cy >= temp.Height) cy = fr_y;
                    fx = x * nWidthFactor - fr_x;
                    fy = y * nHeightFactor - fr_y;
                    nx = 1.0 - fx;
                    ny = 1.0 - fy;

                    color1 = temp.GetPixel(fr_x, fr_y);
                    color2 = temp.GetPixel(cx, fr_y);
                    color3 = temp.GetPixel(fr_x, cy);
                    color4 = temp.GetPixel(cx, cy);

                    // Blue
                    bp1 = (byte)(nx * color1.B + fx * color2.B);

                    bp2 = (byte)(nx * color3.B + fx * color4.B);

                    nBlue = (byte)(ny * (double)(bp1) + fy * (double)(bp2));

                    // Green
                    bp1 = (byte)(nx * color1.G + fx * color2.G);

                    bp2 = (byte)(nx * color3.G + fx * color4.G);

                    nGreen = (byte)(ny * (double)(bp1) + fy * (double)(bp2));

                    // Red
                    bp1 = (byte)(nx * color1.R + fx * color2.R);

                    bp2 = (byte)(nx * color3.R + fx * color4.R);

                    nRed = (byte)(ny * (double)(bp1) + fy * (double)(bp2));

                    bmap.SetPixel(x, y, System.Drawing.Color.FromArgb
            (255, nRed, nGreen, nBlue));
                }
            }



            bmap = SetGrayscale(bmap);
            bmap = RemoveNoise(bmap);

            return bmap;

        }


        public Bitmap SetGrayscale(Bitmap img)
        {

            Bitmap temp = (Bitmap)img;
            Bitmap bmap = (Bitmap)temp.Clone();
            Color c;
            for (int i = 0; i < bmap.Width; i++)
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    c = bmap.GetPixel(i, j);
                    byte gray = (byte)(0.3 * c.R + 0.59 * c.G + 0.11 * c.B);

                    bmap.SetPixel(i, j, Color.FromArgb(gray, gray, gray));
                }
            }
            return (Bitmap)bmap.Clone();

        }

        public Bitmap RemoveNoise(Bitmap bmap)
        {

            for (var x = 0; x < bmap.Width; x++)
            {
                for (var y = 0; y < bmap.Height; y++)
                {
                    var pixel = bmap.GetPixel(x, y);
                    if (pixel.R < 162 && pixel.G < 162 && pixel.B < 162)
                        bmap.SetPixel(x, y, Color.Black);
                    else if (pixel.R > 162 && pixel.G > 162 && pixel.B > 162)
                        bmap.SetPixel(x, y, Color.White);
                }
            }

            return bmap;
        }

        private string ExtractCode(string cardText, int photographerId)
        {
            //sometimes the string has some strange characteres added
            var onlyLettersAndNumbers = new string(cardText.Where(c=>Char.IsLetter(c) || Char.IsNumber(c)).ToArray()).ToUpper();

            string[] numbers = Regex.Split(onlyLettersAndNumbers,@"\D+");
            
            foreach (var l in numbers)
            {
                if(l.Count() == 7)
                {
                    var code = _context.Card.Where(c => c.Code.ToUpper() == l.ToUpper());

                    if (code.Count() > 0)
                    {
                        return code.First().Code;
                    }
                }
            }

            //considering the code is between Photos Of Us and the photographer URL
            //Regex regex = new Regex("PHOTOSOFUS(.*)WWWPHOTOSOFUS");

            //string potencialCode = regex.Match(onlyLettersAndNumbers).Groups[1].ToString();

            
            return string.Empty;  
        }
    }
}
